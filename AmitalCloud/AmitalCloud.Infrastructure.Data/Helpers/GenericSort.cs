using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Linq.Expressions;
using System.Transactions;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class GenericSort
    {
        private readonly IObjectTablePropertyGetter _objectTablePropertyGetter;
        int tenant = 0;
        public GenericSort(IObjectTablePropertyGetter objectTablePropertyGetter, int tenant)
        {
            this.tenant = tenant;
            _objectTablePropertyGetter = objectTablePropertyGetter;
        }

        public GenericSort(IObjectTablePropertyGetter objectTablePropertyGetter)
        {
            _objectTablePropertyGetter = objectTablePropertyGetter;
        }
        public GenericSort() { }

        public IQueryable<T> GetSorterQuery<T, N>(QueryOperations queryOperations, IQueryable<T> querableData)
        {
            string keyName = null;
            if (!string.IsNullOrEmpty(queryOperations.ObjectTableName))
            {
                throw new NotImplementedException("GetObjectTableKeyName is not implemented");
                // keyName = GetObjectTableKeyName(queryOperations);
            }
            SortParams<T, N> sortParams = new SortParams<T, N>();
            sortParams.QuerableData = querableData;
            sortParams.SortDirection = queryOperations.SortDirection;
            sortParams.FirstSortExpression = GetSortExpression<T, N>(queryOperations.SortByColumnName);
            if (!String.IsNullOrWhiteSpace(keyName) && //AmitalCloudSettings.DatabaseManagementSystem == "oracle" &&
                typeof(T).GetProperty(keyName).PropertyType == typeof(Guid)
                && AmitalCloudSettings.IsCostomsDeploy // crush on sql server on branch amital - fast respone 
                )
            {
                return GetSortedQuery<T, N>(sortParams);
            }
            if (!string.IsNullOrEmpty(keyName))
            {
                sortParams.SecondarySortByField = keyName;
                sortParams.SecondSortExpression = GetSortExpression<T, string>(keyName);
                return GetSortedQueryWithSecondarySort<T, N>(sortParams);
            }
            else
            {
                return GetSortedQuery<T, N>(sortParams);
            }

        }
        private IQueryable<T> GetSortedQuery<T, N>(SortParams<T, N> sortParams)
        {
            if (sortParams.SortDirection.ToLower() == "ascending")
            {
                return sortParams.QuerableData.AsQueryable<T>().OrderBy<T, N>(sortParams.FirstSortExpression);
            }
            else
            {
                return sortParams.QuerableData.AsQueryable<T>().OrderByDescending<T, N>(sortParams.FirstSortExpression);
            }
        }
        private IQueryable<T> GetSortedQueryWithSecondarySort<T, N>(SortParams<T, N> sortParams)
        {
            if (sortParams.SortDirection.ToLower() == "ascending")
            {
                return sortParams.QuerableData.AsQueryable<T>().OrderBy<T, N>(sortParams.FirstSortExpression).ThenBy<T, string>(sortParams.SecondSortExpression);
            }
            else
            {
                return sortParams.QuerableData.AsQueryable<T>().OrderByDescending<T, N>(sortParams.FirstSortExpression).ThenByDescending(sortParams.SecondSortExpression);
            }
        }

        private Expression<Func<T, N>> GetSortExpression<T, N>(string sortByFieldName)
        {
            var param = Expression.Parameter(typeof(T), "item");
            var sortExpression = Expression.Lambda<Func<T, N>>
               (Expression.Convert(Expression.Property(param, sortByFieldName), typeof(N)), param);
            return sortExpression;
        }

        private string GetObjectTableKeyName(QueryOperations queryOperations, int tenant = 0)
        {
            string keyName = null;
            using (TransactionScope scope = TransactionFactory.GetNewTransaction())
            {
                keyName = _objectTablePropertyGetter.GetKeyPropertyPath(queryOperations.ObjectTableName, tenant);
                scope.Complete();
            }
            return keyName;
        }


    }

    public class SortParams<T, N>
    {
        public string SecondarySortByField { get; set; }
        public Expression<Func<T, N>> FirstSortExpression { get; set; }
        public Expression<Func<T, string>> SecondSortExpression { get; set; }
        public IQueryable<T> QuerableData { get; set; }
        public string SortDirection { get; set; }
    }
}
