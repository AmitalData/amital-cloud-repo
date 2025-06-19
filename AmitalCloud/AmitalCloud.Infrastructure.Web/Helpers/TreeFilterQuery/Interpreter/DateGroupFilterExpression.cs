using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using AmitalCloud.Infrastructure.Domain.DataContracts;

namespace AmitalCloud.Infrastructure.Web.Helpers.TreeFilterQuery
{
    public class DateGroupFilterExpression : IQueryTreeFilterExpression
    {

        public HashSet<string> ValidOperator = new HashSet<string>
        {
        "NotEqual",
        "Equal",
        "Previous",
        "Current",
        "Next",
        "Between",
        };

        public void Interpret(QueryTreeFilterContext queryTreeFilterContext)
        {
            QueryTreeFilterIterator iterator = CreateIterator(queryTreeFilterContext);
            if (!iterator.Any()) return;
            while (iterator.HasNext())
            {
                Handle(iterator.Next());
            }
        }
        private void Handle(QueryFilterItem filterItem)
        {

            HandleDateGroupFilter(filterItem);
        }

        private void HandleDateGroupFilter(QueryFilterItem queryFilterItem)
        {
            if (!ValidOperator.Contains(queryFilterItem.Operator))
                return;
            QueryFilterItem queryFilterItemLessThan = CloneQueryFilterItem(queryFilterItem);
            QueryFilterItem queryFilterItemGreaterThan = CloneQueryFilterItem(queryFilterItem);
            switch (queryFilterItem.Operator)
            {
                case "NotEqual":
                    HandelNotEqualOperator(queryFilterItem, queryFilterItemLessThan, queryFilterItemGreaterThan);
                    break;
                case "Equal":
                    HandelEqualOperator(queryFilterItem, queryFilterItemLessThan, queryFilterItemGreaterThan);
                    break;
                case "Previous": 
                case "Next":
                    HandelCalculatedOperator(queryFilterItem, queryFilterItemLessThan, queryFilterItemGreaterThan);
                    break;
                case "Current":
                    queryFilterItem.FieldValue3 = 1;
                    HandelCalculatedOperator(queryFilterItem, queryFilterItemLessThan, queryFilterItemGreaterThan);
                    break;
                case "Between":
                    HandelBetweenOperator(queryFilterItem, queryFilterItemLessThan, queryFilterItemGreaterThan);
                    break;

            }

            ChangeToGroup(queryFilterItem);

            queryFilterItem.QueryFilterItems.AddRange(new[] { queryFilterItemLessThan, queryFilterItemGreaterThan });
        }

        private void ChangeToGroup(QueryFilterItem queryFilterItem)
        {
            queryFilterItem.FieldName = null;
            queryFilterItem.FieldValue = null;
            queryFilterItem.FieldValue2 = null;
            queryFilterItem.FieldValue3 = null;
            queryFilterItem.Operator = null;
            queryFilterItem.IsCustom = false;
            queryFilterItem.DisplayInList = false;
            queryFilterItem.IsCustomField = false;
            queryFilterItem.FieldDataType = null;
            queryFilterItem.FilterType = "And";
            queryFilterItem.DateGroupCode = null;
            queryFilterItem.IsListFilter = false;
            queryFilterItem.IsAnalyticsMetadatas = false;
            queryFilterItem.QueryFilterItems = new List<QueryFilterItem>();
        }

        private void HandelEqualOperator(QueryFilterItem queryFilterItem, QueryFilterItem queryFilterItemLessThan, QueryFilterItem queryFilterItemGreaterThan)
        {
            queryFilterItemGreaterThan.Operator = "GreaterThanOrEqual";
            queryFilterItemLessThan.Operator = "LessThan";
            var date = DateTime.Parse(queryFilterItemLessThan.FieldValue.ToString());
            ConvertDateByDateGroupCode(ref date, queryFilterItem.DateGroupCode);
            queryFilterItemLessThan.FieldValue = date;
        }

        private void HandelNotEqualOperator(QueryFilterItem queryFilterItem, QueryFilterItem queryFilterItemLessThan, QueryFilterItem queryFilterItemGreaterThan)
        {
            queryFilterItemLessThan.Operator = "LessThan";
            queryFilterItemGreaterThan.Operator = "GreaterThanOrEqual";
            var date = DateTime.Parse(queryFilterItemGreaterThan.FieldValue.ToString());
            ConvertDateByDateGroupCode(ref date, queryFilterItem.DateGroupCode);
            queryFilterItemGreaterThan.FieldValue = date;
        }

        private void HandelBetweenOperator(QueryFilterItem queryFilterItem, QueryFilterItem queryFilterItemLessThan, QueryFilterItem queryFilterItemGreaterThan)
        {
            queryFilterItemLessThan.Operator = "LessThan";
            queryFilterItemGreaterThan.Operator = "GreaterThanOrEqual";
            var startDate = DateTime.Parse(queryFilterItemGreaterThan.FieldValue.ToString());
            var endDate = DateTime.Parse(queryFilterItemGreaterThan.FieldValue2.ToString());
            queryFilterItemLessThan.FieldValue = endDate.Date.AddDays(1);
            queryFilterItemGreaterThan.FieldValue = startDate.Date;
        }

        private void HandelCalculatedOperator(QueryFilterItem queryFilterItem, QueryFilterItem queryFilterItemLessThan, QueryFilterItem queryFilterItemGreaterThan)
        {
            queryFilterItemLessThan.Operator = "LessThan";
            queryFilterItemGreaterThan.Operator = "GreaterThanOrEqual";


            var number = int.Parse(queryFilterItem.FieldValue3.ToString());

            DateTime startDate = GetStartDateByDateGroupCode(queryFilterItem, number);
            DateTime endDate;
            if (queryFilterItem.Operator == "Previous")
            {
                endDate = ConvertDateByDateGroupCode(ref startDate, queryFilterItem.DateGroupCode, number * -1);
                queryFilterItemLessThan.FieldValue = startDate;
                queryFilterItemGreaterThan.FieldValue = endDate;
            }
            else { 
                endDate = ConvertDateByDateGroupCode(ref startDate, queryFilterItem.DateGroupCode, number);
                queryFilterItemLessThan.FieldValue = endDate;
                queryFilterItemGreaterThan.FieldValue = startDate;
            }
        }

        private DateTime GetStartDateByDateGroupCode(QueryFilterItem queryFilterItem, int factor)
        {
            switch (queryFilterItem.DateGroupCode?.Trim())
            {
                case "Day":
                    return DateTime.Now.Date;
                case "Week":
                    return StartOfWeek(DateTime.Now.Date);
                case "Month":
                    return new DateTime(DateTime.Now.Year, DateTime.Now.Month,1);
                case "Year":
                    return new DateTime(DateTime.Now.Year, 1, 1);
                case "Quarter":
                    int quarterStartMonth = (int)(Math.Ceiling(DateTime.Now.Month / 3.0) * 3 - 2);
                    return new DateTime(DateTime.Now.Year, quarterStartMonth, 1);
                default:
                    return DateTime.Now.Date;
            }
        }
        private DateTime StartOfWeek(DateTime dt)
        {
            var culture = CultureInfo.CurrentCulture;
            int diff = (7 + (dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        private DateTime ConvertDateByDateGroupCode(ref DateTime date, string dateGroupCode, int factor = 1)
        {
            switch (dateGroupCode)
            {
                case "Day":
                    return date.AddDays(1 * factor);
                case "Week":
                    return date.AddDays(7 * factor);
                case "Month":
                    return date.AddMonths(1 * factor);
                case "Year":
                    return date.AddYears(1 * factor);
                case "Quarter":
                    return date.AddMonths(3 * factor);
                default:
                    return date;
            }
        }

        private QueryFilterItem CloneQueryFilterItem(QueryFilterItem queryFilterItem)
        {
            return new QueryFilterItem
            {
                DisplayInList = queryFilterItem.DisplayInList,
                FieldDataType = queryFilterItem.FieldDataType,
                FieldName = queryFilterItem.FieldName,
                FieldValue2 = queryFilterItem.FieldValue2,
                FilterType = queryFilterItem.FilterType,
                FieldValue = queryFilterItem.FieldValue,
                FieldValue3 = queryFilterItem.FieldValue3,
                IsAnalyticsMetadatas = queryFilterItem.IsAnalyticsMetadatas,
                IsCustom = queryFilterItem.IsCustom,
                IsCustomField = queryFilterItem.IsCustomField,
                IsListFilter = queryFilterItem.IsListFilter,
                Operator = queryFilterItem.Operator
            };
        }

        private QueryTreeFilterIterator CreateIterator(QueryTreeFilterContext queryTreeFilterContext)
        {
            var iterator = new QueryTreeFilterCollection(queryTreeFilterContext).CreateIterator();
            var collection = iterator.Collection.Where(d => !string.IsNullOrEmpty(d.DateGroupCode) ||
                (d.IsAnalyticsMetadatas &&  d.Operator == "Between")).ToList();
            iterator.SetCollection(collection);
            return iterator;
        }
    }
}
