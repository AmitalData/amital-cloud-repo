using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace AmitalCloud.Infrastructure.Application.Helpers
{
    public class EntityListFilter
    {
        public static QueryOperations GetQueryOperations(byte[] xmlFilters)
        {
            MemoryStream memorystream = new MemoryStream(xmlFilters);
            XmlSerializer serializer = new XmlSerializer(typeof(QueryOperations));
            QueryOperations queryOperations = (QueryOperations)serializer.Deserialize(memorystream);
            return queryOperations;
        }
        public static IQueryable<T> ApplyEntityNonListFilters<T>(QueryOperations queryOperations, IQueryable<T> iQueryable)
        {
            GenericFilter filter = new GenericFilter();
            QueryOperations nonListQueryOperation = new QueryOperations();
            nonListQueryOperation.QueryFilterItems = queryOperations.QueryFilterItems.Where(d => !d.DisplayInList).ToList();
            iQueryable = filter.GetFilteredQuery<T>(nonListQueryOperation, iQueryable);
            return iQueryable;
        }
        public static IQueryable<T> ApplyEntityListFilters<T>(QueryOperations queryOperations, IQueryable<T> iQueryable)
        {
            GenericFilter filter = new GenericFilter();
            QueryOperations listQueryOperation = new QueryOperations();
            listQueryOperation.QueryFilterItems = queryOperations.QueryFilterItems.Where(d => d.DisplayInList).ToList();
            iQueryable = filter.GetFilteredQuery<T>(listQueryOperation, iQueryable);
            return iQueryable;
        }
    }
}
