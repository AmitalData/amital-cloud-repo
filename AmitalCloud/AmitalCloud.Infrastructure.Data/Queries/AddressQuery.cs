using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AmitalCloud.Infrastructure.Data.EntityDataMappings;

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
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new AddressDataMapping()));
            var mapper = config.CreateMapper();
            return mapper.Map<AddressPM>(entity);
        }

    }
}