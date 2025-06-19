using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Data.Services;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class CardQuery
    {
        CardRepository repository;
        public CardQuery(int tenant) : this(new CardRepository(tenant))
        {
        }
        public CardQuery(CardRepository cardRepository)
        {
            repository = cardRepository;
        }
        public CardPM GetSinglePMFromCache(string id, int tenant)
        {
            string entityKeyString = $"GetSinglePMFromCache({id},{tenant})";
            return CacheManager.GetOrInsertNewObject<CardPM>(entityKeyString, () => this.GetSinglePM(id, tenant));
        }

        public CardPM GetSinglePM(string id, int tenant)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string entityKeyString = $"CardPM({id},{tenant})";
                CardPM entity;
                var repository = new Repository<Address>(tenant);
                var addresslist = repository.GetMulti(a => a.Tenant == tenant && a.CardId == id, a => new AddressPM(a), "Country,State").ToList();
                string myMainAddressId = addresslist.Where(a => a.AddressTypeId.ToUpper() == "M").FirstOrDefault().Id;
                string myBillingAddressId = addresslist.Where(a => a.AddressTypeId.ToUpper() == "B").FirstOrDefault().Id;
                string myPickupDeliveryAddressId = addresslist.Where(a => a.AddressTypeId.ToUpper() == "P").FirstOrDefault().Id;
                if (HttpContextHelper.HttpContext != null)
                {
                    entity = (CardPM)CacheManager.CacheWrapper.Get(entityKeyString);
                    if (entity == null)
                    {
                        entity = GetEntity(id, tenant, myMainAddressId, myBillingAddressId, myPickupDeliveryAddressId);
                        CacheManager.CacheWrapper.Insert(entityKeyString, entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                    }
                }
                else
                {
                    entity = GetEntity(id, tenant, myMainAddressId, myBillingAddressId, myPickupDeliveryAddressId);
                }
                entity = Set(entity, tenant);
                return entity;
            }

            return null;
        }

        private CardPM GetEntity(string id, int tenant, string myMainAddressId, string myBillingAddressId, string myPickupDeliveryAddressId)
        {
            CardPM entity = new Repository<Card>(tenant).GetMulti(a => a.Id == id, a => GetNewPM(myMainAddressId, myBillingAddressId, myPickupDeliveryAddressId, a), "Customer,SalesmanUser,PartnerType,SharedLogisticsInvitationStatus,Airline").FirstOrDefault();
            if (entity != null)
            {
                entity.Addresses = new AddressQuery(tenant).GetAddressesByCardId(id, tenant);
                new EntityCustomFieldService(new EntityCustomFieldServiceArgs() { ObjectTableName = "Card", Tenant = tenant, Type = "PM", Entities = new List<CardPM> { entity }.Cast<object>().ToList() }).Set();
            }
            return entity;
        }
        private CardPM GetNewPM(string myMainAddressId, string myBillingAddressId, string myPickupDeliveryAddressId, Card a)
            => new CardPM(a)
            {
                //PartnerTypeName = a.PartnerType == null ? null : a.PartnerType.Name,
                //MainAddressId = myMainAddressId,
                //BillingAddressId = myBillingAddressId,
                //PickupDeliveryAddressId = myPickupDeliveryAddressId,
                //ComputedLocalName = string.IsNullOrEmpty(a.LocalName) ? a.EnglishName : a.LocalName,
                //ClassifierName = a.ClassifierUser != null ? a.ClassifierUser.Contact.EnglishName : "",
                //CollectorName = a.CollectorUser != null ? a.CollectorUser.Contact.EnglishName : "",
                EnableConsolidationInvoices = a.EnableConsolidationInvoices,
                SalesmanUserId = a.Customer != null ? a.Customer.SalesmanUserId : null,
                //AccountManagerUserId = a.Customer == null ? null : a.Customer.AccountManagerUserId,
                //TeamId = a.Customer == null ? null : a.Customer.TeamId,
                //SalesmanBusinessUnitId = a.Customer == null ? null : (a.Customer.SalesmanUser == null ? null : a.Customer.SalesmanUser.BusinessUnitId),
                //CustomerStatusCode = a.Customer != null ? (a.Customer.CustomerStatus != null ? a.Customer.CustomerStatus.Code : null) : null,
                //RankId = a.Customer != null ? (a.Customer.Rank != null ? a.Customer.Rank.Id : null) : null,
                //IndustryId = a.Customer != null ? (a.Customer.Industry != null ? a.Customer.Industry.Id : null) : null,
                //LeadSourceId = a.Customer != null ? (a.Customer.LeadSource != null ? a.Customer.LeadSource.Id : null) : null,
                //LeadDescription = a.Customer != null ? a.Customer.LeadDescription : null,
                //StartWorkingDate = a.Customer != null ? a.Customer.StartWorkingDate : null,
                //ICAO = a.Airline ? .ICAO ,
                //CustomerSizeId = a.Customer != null ? (a.Customer.CustomerSize != null ? a.Customer.CustomerSize.Id : null) : null,
            };
        private CardPM Set(CardPM card, int tenant)
        {
            List<DocumentTypeList> documentTypeLists =
                new DocumentTypeQuery(tenant).GetDocumentTypeListsByObjectTableId(ObjectTableRepository.GetObjectTableByName("ARInvoice"), tenant);
            card.SingleInvoiceTemplateId = (card.SingleInvoiceTemplateId == null) ? GetDefaultDocumentTypeTemplateId("999S", documentTypeLists) : card.SingleInvoiceTemplateId;
            card.CustomsInvoiceTemplateId = (card.CustomsInvoiceTemplateId == null) ? GetDefaultDocumentTypeTemplateId("999CI", documentTypeLists) : card.CustomsInvoiceTemplateId;
            card.ConsolidationInvoiceTemplateId = (card.ConsolidationInvoiceTemplateId == null) ? GetDefaultDocumentTypeTemplateId("999C", documentTypeLists) : card.ConsolidationInvoiceTemplateId;
            card.ManifestInvoiceTemplateId = (card.ManifestInvoiceTemplateId == null) ? GetDefaultDocumentTypeTemplateId("999M", documentTypeLists) : card.ManifestInvoiceTemplateId;
            return card;
        }

        private string GetDefaultDocumentTypeTemplateId(string documentTypeCode, List<DocumentTypeList> documentTypeLists)
        {
            var documentTypeList = documentTypeLists.Where(d => d.Code == documentTypeCode).FirstOrDefault();
            return documentTypeList?.DocumentTypeDefaultReportTemplateId;
        }
    }
}
