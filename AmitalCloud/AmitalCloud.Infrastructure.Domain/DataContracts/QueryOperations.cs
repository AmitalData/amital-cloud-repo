using System.Collections.Generic;
using System.Linq;

namespace AmitalCloud.Infrastructure.Domain.DataContracts
{
    public class QueryOperations
    {
        public QueryOperations()
        {
            PageIndex = 1;
            PageSize = 10;
            DataCount = 0;
            QueryFilterItems = new List<QueryFilterItem>();
        }

        public List<QueryFilterItem> QueryFilterItems { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int DataCount { get; set; }
        public string SortByColumnName { get; set; }
        public string SortDirection { get; set; }
        string querySection;
        public bool GetAll { get; set; }

        public string UserId { get; set; }
        public string ObjectTableName { get; set; }

        public string QuerySection
        {
            get { return querySection; }
            set { querySection = value; }
        }

        public void SetFilter(string name, object value, bool isCustom, string Operator, object value2, bool displayInList)
        {
            if (QueryFilterItems == null)
            {
                QueryFilterItems = new List<QueryFilterItem>();
                if (Operator == "NoDate")
                {
                    QueryFilterItem item = new QueryFilterItem() { FieldName = name, FieldValue = null, IsCustom = isCustom, Operator = "Equal", DisplayInList = displayInList };
                    QueryFilterItems.Add(item);
                }
                else
                {
                    if (value != null)
                    {
                        QueryFilterItem item = new QueryFilterItem() { FieldName = name, FieldValue = value, IsCustom = isCustom, Operator = Operator, FieldValue2 = value2, DisplayInList = displayInList };
                        QueryFilterItems.Add(item);

                    }
                }
            }
            else
            {

                QueryFilterItem item = QueryFilterItems.Where(d => d.FieldName == name).FirstOrDefault();
                if (item != null)
                {
                    if (Operator == "NoDate")
                    {
                        item.Operator = "Equal";
                        item.DisplayInList = displayInList;
                        item.FieldValue = null;
                    }
                    else
                    {
                        item.Operator = Operator;
                        item.DisplayInList = displayInList;

                        if (value == null)
                        {
                            if (Operator == "Between")
                            {
                                if (value2 == null)
                                {
                                    QueryFilterItems.Remove(item);
                                }
                            }
                            else
                            {
                                QueryFilterItems.Remove(item);
                            }
                        }
                        else
                        {

                            if (item.FieldValue != value)
                            {
                                item.FieldValue = value;
                            }

                            if (item.FieldValue2 != value2)
                            {
                                item.FieldValue2 = value2;
                            }
                            if (value.ToString() == "null")
                            {
                                item.FieldValue = null;
                            }
                        }
                    }
                }
                else
                {
                    if (Operator == "NoDate")
                    {
                        QueryFilterItem newItem = new QueryFilterItem() { FieldName = name, FieldValue = null, IsCustom = isCustom, Operator = "Equal", DisplayInList = displayInList };
                        QueryFilterItems.Add(newItem);
                    }
                    else
                    {
                        if (value != null)
                        {
                            QueryFilterItem newItem = new QueryFilterItem() { FieldName = name, FieldValue = (value.ToString() == "null" ? null : value), IsCustom = isCustom, Operator = Operator, FieldValue2 = value2, DisplayInList = displayInList };
                            QueryFilterItems.Add(newItem);
                        }
                    }

                }

            }
        }

        public void SetFilter(string name, object value, bool isCustom, string Operator, object value2, object value3, bool displayInList)
        {
            if (QueryFilterItems == null)
            {
                QueryFilterItems = new List<QueryFilterItem>();
                if (value != null)
                {
                    QueryFilterItem item = new QueryFilterItem() { FieldName = name, FieldValue = value, IsCustom = isCustom, Operator = Operator, FieldValue2 = value2, DisplayInList = displayInList, FieldValue3 = value3 };
                    QueryFilterItems.Add(item);

                }
                else
                {
                    QueryFilterItem item = new QueryFilterItem() { FieldName = name, IsCustom = isCustom, Operator = Operator, FieldValue2 = value2, DisplayInList = displayInList, FieldValue3 = value3 };
                    QueryFilterItems.Add(item);
                }
            }
            else
            {
                QueryFilterItem item = QueryFilterItems.Where(d => d.FieldName == name).FirstOrDefault();
                if (item != null)
                {
                    item.Operator = Operator;
                    item.DisplayInList = displayInList;

                    if (value == null)
                    {
                        if (Operator == "Between")
                        {
                            if (value2 == null)
                            {
                                QueryFilterItems.Remove(item);
                            }
                        }
                        else
                        {
                            QueryFilterItems.Remove(item);
                        }
                    }
                    else
                    {
                        if (item.FieldValue != value)
                        {
                            item.FieldValue = value;
                        }

                        if (item.FieldValue2 != value2)
                        {
                            item.FieldValue2 = value2;
                        }

                        if (item.FieldValue3 != value3)
                        {
                            item.FieldValue3 = value3;
                        }
                    }
                }
                else
                {
                    if (value != null)
                    {
                        QueryFilterItem newItem = new QueryFilterItem() { FieldName = name, FieldValue = value, IsCustom = isCustom, Operator = Operator, FieldValue2 = value2, DisplayInList = displayInList, FieldValue3 = value3 };
                        QueryFilterItems.Add(newItem);
                    }
                }
            }
        }

        public void SetFilter(string name, object value, bool isCustom, string Operator, object value2, bool displayInList, bool isCustomField, string fieldDataType, bool isListFilter = false)
        {
            if (QueryFilterItems == null)
            {
                QueryFilterItems = new List<QueryFilterItem>();
                if (Operator == "NoDate")
                {
                    QueryFilterItem item = new QueryFilterItem() { FieldName = name, FieldValue = null, IsCustom = isCustom, Operator = "Equal", DisplayInList = displayInList, IsListFilter = isListFilter };
                    QueryFilterItems.Add(item);
                }
                else
                {
                    if (value != null)
                    {
                        QueryFilterItem item = new QueryFilterItem() { FieldName = name, FieldValue = value, IsCustom = isCustom, Operator = Operator, FieldValue2 = value2, DisplayInList = displayInList, IsCustomField = isCustomField, FieldDataType = fieldDataType, IsListFilter = isListFilter };
                        QueryFilterItems.Add(item);

                    }
                }
            }
            else
            {

                QueryFilterItem item = QueryFilterItems.Where(d => d.FieldName == name).FirstOrDefault();
                if (item != null)
                {
                    item.IsListFilter = isListFilter;
                    if (Operator == "NoDate")
                    {
                        item.Operator = "Equal";
                        item.DisplayInList = displayInList;
                        item.FieldValue = null;
                    }
                    else
                    {
                        item.Operator = Operator;
                        item.DisplayInList = displayInList;

                        if (value == null)
                        {
                            if (Operator == "Between")
                            {
                                if (value2 == null)
                                {
                                    QueryFilterItems.Remove(item);
                                }
                            }
                            else
                            {
                                QueryFilterItems.Remove(item);
                            }
                        }
                        else
                        {
                            if (item.FieldValue != value)
                            {
                                item.FieldValue = value;
                            }

                            if (item.FieldValue2 != value2)
                            {
                                item.FieldValue2 = value2;
                            }
                        }
                    }
                }
                else
                {
                    if (Operator == "NoDate")
                    {
                        QueryFilterItem newItem = new QueryFilterItem() { FieldName = name, FieldValue = null, IsCustom = isCustom, Operator = "Equal", DisplayInList = displayInList, IsListFilter = isListFilter };
                        QueryFilterItems.Add(newItem);
                    }
                    else
                    {
                        if (value != null)
                        {

                            QueryFilterItem newItem = new QueryFilterItem() { FieldName = name, FieldValue = value, IsCustom = isCustom, Operator = Operator, FieldValue2 = value2, DisplayInList = displayInList, IsCustomField = isCustomField, FieldDataType = fieldDataType, IsListFilter = isListFilter };
                            QueryFilterItems.Add(newItem);
                        }
                    }
                }

            }
        }
    }
}
