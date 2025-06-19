using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class AddressQuery
    {
        IRepository<Address> repository;
        public AddressQuery(int tenant)
        {
            repository = new Repository<Address>(tenant);
        }
        public AddressQuery(IRepository<Address> addressRepository) => repository = addressRepository;
        #region Get Single AddressPM
        public AddressPM GetAddressPMByTypeAndCard(string cardId, string typeId, int tenant) =>
            repository.GetMulti(a => a.Tenant == tenant && a.CardId == cardId && a.AddressTypeId.ToUpper() == typeId.ToUpper(), a => GetNewAddressPM(a), "Country,State").FirstOrDefault();
        #endregion Get Single AddressPM
        #region Get List<AddressList>
        public List<AddressPM> GetAddressesByCardId(string cardId, int tenant) => repository.GetMulti(a => a.Tenant == tenant && a.CardId == cardId, a => GetNewAddressPM(a), "Country,State").ToList();
        #endregion Get List<AddressList>
        private AddressPM GetNewAddressPM(Address entity)
        {
            return new AddressPM(entity)
            {
                //CountryCode = entity.Country != null ? entity.Country.Code : null,
                //CountryEnglishName = entity.Country != null ? entity.Country.EnglishName : null,
                //CountryName = entity.Country != null ? (entity.IsLocalLanguage ? entity.Country.LocalName : entity.Country.EnglishName) : null,
                //StateCode = entity.State != null ? entity.State.Code : null,
                //StateEnglishName = entity.State != null ? entity.State.EnglishName : null,
                //HasStates = entity.Country == null ? false : entity.Country.HasStates,
                //IsStateRequired = entity.Country == null ? false : entity.Country.IsStateRequired,
            };
        }

    }
}