using AmitalCloud.Infrastructure.Domain.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class GenericFilter
    {
        CustomFieldClass customFilterClass = new CustomFieldClass();
        public IQueryable<T> GetFilteredQuery<T>(QueryOperations operations, IQueryable<T> queryableData)
        {
            List<QueryFilterItem> queryFilters = operations.QueryFilterItems;

            if (queryFilters.Count != 0)
            {
                Expression grandExpression = null;
                ParameterExpression pe = Expression.Parameter(typeof(T), "item");
                foreach (QueryFilterItem item in queryFilters)
                {
                    if (AmitalCloudSettings.WorkEnvironment == "customs")
                    {
                        if (item.FieldName == "SearchFields")
                        {
                            item.FieldValue = item.FieldValue.ToString().ToLower();
                        }
                    }

                    if (!item.IsCustom)
                    {
                        GetCustomFieldStringValue(item);

                        if (item.Operator != null)
                        {
                            switch (item.Operator)
                            {
                                case "LargerThan":
                                    {

                                        if (item.IsCustomField)
                                        {
                                            Expression left = Expression.Property(pe, typeof(T).GetProperty(item.FieldName));
                                            Expression right = Expression.Constant(item.FieldValue, left.Type);
                                            MethodInfo mi = typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) });

                                            MemberExpression field = Expression.PropertyOrField(pe, item.FieldName);

                                            MethodCallExpression compareExpression = Expression.Call(field, mi, Expression.Constant(item.FieldValue));
                                            Expression zeroExpression = Expression.Constant(0, typeof(int));

                                            if (grandExpression == null)
                                            {

                                                grandExpression = System.Linq.Expressions.Expression.GreaterThan(compareExpression, zeroExpression);
                                            }
                                            else
                                            {
                                                System.Linq.Expressions.Expression e1 = System.Linq.Expressions.Expression.GreaterThan(compareExpression, zeroExpression);
                                                grandExpression = System.Linq.Expressions.Expression.And(grandExpression, e1);
                                            }

                                        }
                                        else
                                        {
                                            Expression left = Expression.Property(pe, typeof(T).GetProperty(item.FieldName));
                                            Expression right = Expression.Constant(item.FieldValue, left.Type);//typeof(double?)

                                            if (grandExpression == null)
                                            {

                                                grandExpression = Expression.GreaterThan(left, right);
                                            }
                                            else
                                            {
                                                Expression e1 = Expression.GreaterThan(left, right);
                                                grandExpression = Expression.And(grandExpression, e1);
                                            }
                                        }
                                        break;
                                    }
                                case "GreaterThanOrEqual":
                                    {
                                        if (item.IsCustomField)
                                        {
                                            System.Linq.Expressions.Expression left = System.Linq.Expressions.Expression.Property(pe, typeof(T).GetProperty(item.FieldName));
                                            System.Linq.Expressions.Expression right = System.Linq.Expressions.Expression.Constant(item.FieldValue, left.Type);
                                            MethodInfo mi = typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) });

                                            MemberExpression field = Expression.PropertyOrField(pe, item.FieldName);

                                            MethodCallExpression compareExpression = Expression.Call(field, mi, Expression.Constant(item.FieldValue));
                                            System.Linq.Expressions.Expression zeroExpression = System.Linq.Expressions.Expression.Constant(0, typeof(int));

                                            if (grandExpression == null)
                                            {

                                                grandExpression = System.Linq.Expressions.Expression.GreaterThanOrEqual(compareExpression, zeroExpression);
                                            }
                                            else
                                            {
                                                System.Linq.Expressions.Expression e1 = System.Linq.Expressions.Expression.GreaterThanOrEqual(compareExpression, zeroExpression);
                                                grandExpression = System.Linq.Expressions.Expression.And(grandExpression, e1);
                                            }

                                        }
                                        else
                                        {
                                            System.Linq.Expressions.Expression left = System.Linq.Expressions.Expression.Property(pe, typeof(T).GetProperty(item.FieldName));
                                            System.Linq.Expressions.Expression right = System.Linq.Expressions.Expression.Constant(item.FieldValue, left.Type);
                                            if (grandExpression == null)
                                            {

                                                grandExpression = System.Linq.Expressions.Expression.GreaterThanOrEqual(left, right);
                                            }
                                            else
                                            {
                                                System.Linq.Expressions.Expression e1 = System.Linq.Expressions.Expression.GreaterThanOrEqual(left, right);
                                                grandExpression = System.Linq.Expressions.Expression.And(grandExpression, e1);
                                            }
                                        }
                                        break;
                                    }

                                case "LessThan":
                                    {
                                        if (item.IsCustomField)
                                        {
                                            System.Linq.Expressions.Expression left = System.Linq.Expressions.Expression.Property(pe, typeof(T).GetProperty(item.FieldName));
                                            System.Linq.Expressions.Expression right = System.Linq.Expressions.Expression.Constant(item.FieldValue, left.Type);
                                            MethodInfo mi = typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) });

                                            MemberExpression field = Expression.PropertyOrField(pe, item.FieldName);

                                            MethodCallExpression compareExpression = Expression.Call(field, mi, Expression.Constant(item.FieldValue));
                                            System.Linq.Expressions.Expression zeroExpression = System.Linq.Expressions.Expression.Constant(0, typeof(int));

                                            if (grandExpression == null)
                                            {

                                                grandExpression = System.Linq.Expressions.Expression.LessThan(compareExpression, zeroExpression);
                                            }
                                            else
                                            {
                                                System.Linq.Expressions.Expression e1 = System.Linq.Expressions.Expression.LessThan(compareExpression, zeroExpression);
                                                grandExpression = System.Linq.Expressions.Expression.And(grandExpression, e1);
                                            }

                                        }
                                        else
                                        {
                                            System.Linq.Expressions.Expression left = System.Linq.Expressions.Expression.Property(pe, typeof(T).GetProperty(item.FieldName));
                                            System.Linq.Expressions.Expression right = System.Linq.Expressions.Expression.Constant(item.FieldValue, left.Type);
                                            if (grandExpression == null)
                                            {

                                                grandExpression = System.Linq.Expressions.Expression.LessThan(left, right);
                                            }
                                            else
                                            {
                                                System.Linq.Expressions.Expression e1 = System.Linq.Expressions.Expression.LessThan(left, right);
                                                grandExpression = System.Linq.Expressions.Expression.And(grandExpression, e1);
                                            }
                                        }
                                        break;
                                    }
                                case "LessThanOrEqual":
                                    {
                                        if (item.IsCustomField)
                                        {
                                            System.Linq.Expressions.Expression left = System.Linq.Expressions.Expression.Property(pe, typeof(T).GetProperty(item.FieldName));
                                            System.Linq.Expressions.Expression right = System.Linq.Expressions.Expression.Constant(item.FieldValue, left.Type);
                                            MethodInfo mi = typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) });

                                            MemberExpression field = Expression.PropertyOrField(pe, item.FieldName);

                                            MethodCallExpression compareExpression = Expression.Call(field, mi, Expression.Constant(item.FieldValue));
                                            System.Linq.Expressions.Expression zeroExpression = System.Linq.Expressions.Expression.Constant(0, typeof(int));

                                            if (grandExpression == null)
                                            {

                                                grandExpression = System.Linq.Expressions.Expression.LessThanOrEqual(compareExpression, zeroExpression);
                                            }
                                            else
                                            {
                                                System.Linq.Expressions.Expression e1 = System.Linq.Expressions.Expression.LessThanOrEqual(compareExpression, zeroExpression);
                                                grandExpression = System.Linq.Expressions.Expression.And(grandExpression, e1);
                                            }

                                        }
                                        else
                                        {
                                            System.Linq.Expressions.Expression left = System.Linq.Expressions.Expression.Property(pe, typeof(T).GetProperty(item.FieldName));
                                            System.Linq.Expressions.Expression right = System.Linq.Expressions.Expression.Constant(item.FieldValue, left.Type);
                                            if (grandExpression == null)
                                            {

                                                grandExpression = System.Linq.Expressions.Expression.LessThanOrEqual(left, right);
                                            }
                                            else
                                            {
                                                System.Linq.Expressions.Expression e1 = System.Linq.Expressions.Expression.LessThanOrEqual(left, right);
                                                grandExpression = System.Linq.Expressions.Expression.And(grandExpression, e1);
                                            }
                                        }
                                        break;
                                    }
                                case "StartsWith":
                                    {
                                        MethodInfo mi = typeof(String).GetMethod("StartsWith", new Type[] { typeof(String) });
                                        MemberExpression field = Expression.PropertyOrField(pe, item.FieldName);
                                        MethodCallExpression startsWith = Expression.Call(field, mi, Expression.Constant(item.FieldValue));

                                        if (grandExpression == null)
                                        {

                                            grandExpression = startsWith;
                                        }
                                        else
                                        {
                                            Expression expression = startsWith;
                                            grandExpression = Expression.And(grandExpression, expression);
                                        }

                                        break;
                                    }
                                case "Contains":
                                    {
                                        MethodInfo mi = typeof(String).GetMethod("Contains", new Type[] { typeof(String) });

                                        MemberExpression field = Expression.PropertyOrField(pe, item.FieldName);

                                        MethodCallExpression startsWith = null;
                                        if (AmitalCloudSettings.WorkEnvironment == "customs")
                                        {
                                            if (item.FieldName == "SearchFields")
                                            {
                                                Expression ex = Expression.Call(field, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                                                startsWith = Expression.Call(ex, mi, Expression.Constant(item.FieldValue));
                                            }
                                            else
                                            {
                                                startsWith = Expression.Call(field, mi, Expression.Constant(item.FieldValue));

                                            }
                                        }
                                        else
                                        {

                                            startsWith = Expression.Call(field, mi, Expression.Constant(item.FieldValue));
                                        }



                                        if (grandExpression == null)
                                        {

                                            grandExpression = startsWith;
                                        }
                                        else
                                        {
                                            Expression expression = startsWith;
                                            grandExpression = Expression.And(grandExpression, expression);
                                        }

                                        break;
                                    }

                                case "InList":
                                    {

                                        string[] listOfValus = item.FieldValue.ToString().Split(',');
                                        Expression inListExpression = null;
                                        foreach (string v in listOfValus)
                                        {

                                            MethodInfo mi = typeof(String).GetMethod("Contains", new Type[] { typeof(String) });

                                            MemberExpression field = System.Linq.Expressions.Expression.PropertyOrField(pe, item.FieldName);

                                            MethodCallExpression contains = System.Linq.Expressions.Expression.Call(field, mi, System.Linq.Expressions.Expression.Constant(v));

                                            if (inListExpression == null)
                                            {
                                                inListExpression = contains;
                                            }
                                            else
                                            {
                                                System.Linq.Expressions.Expression expression = contains;
                                                inListExpression = System.Linq.Expressions.Expression.Or(inListExpression, expression);
                                            }




                                        }
                                        if (grandExpression == null)
                                        {

                                            grandExpression = inListExpression;
                                        }
                                        else
                                        {
                                            System.Linq.Expressions.Expression expression = inListExpression;
                                            grandExpression = System.Linq.Expressions.Expression.And(grandExpression, expression);
                                        }



                                        break;



                                    }

                                case "InListExact":
                                    {
                                        string[] listOfValus = item.FieldValue.ToString().Split(',');
                                        Expression e1 = null;
                                        foreach (string v in listOfValus)
                                        {
                                            MemberExpression field = Expression.PropertyOrField(pe, item.FieldName);
                                            Expression left = Expression.Property(pe, typeof(T).GetProperty(item.FieldName));
                                            Expression right = Expression.Constant(v);
                                            if (v != null && v.ToLower() == "null")
                                            {
                                                right = Expression.Constant(null);
                                            }

                                            if (e1 == null)
                                            {
                                                e1 = Expression.Equal(left, right);
                                            }
                                            else
                                            {
                                                Expression e2 = Expression.Equal(left, right);
                                                e1 = Expression.Or(e1, e2);
                                            }
                                        }

                                        if (grandExpression == null)
                                        {

                                            grandExpression = e1;
                                        }
                                        else
                                        {
                                            Expression expression = e1;
                                            grandExpression = Expression.And(grandExpression, e1);
                                        }

                                        break;
                                    }
                                case "Between":
                                    {
                                        string[] propertyinfo = item.FieldName.Split('.');
                                        Expression middle = Expression.Property(pe, typeof(T).GetProperty(item.FieldName));

                                        if (item.FieldValue != null && item.FieldValue2 != null)
                                        {
                                            if (item.IsCustomField)
                                            {
                                                Expression leftValue = Expression.Constant(item.FieldValue, middle.Type);
                                                Expression rightValue = Expression.Constant(item.FieldValue2, middle.Type);
                                                MethodInfo mi = typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) });
                                                MethodCallExpression leftCompareExpression = Expression.Call(middle, mi, Expression.Constant(item.FieldValue));
                                                MethodCallExpression rightCompareExpression = Expression.Call(middle, mi, Expression.Constant(item.FieldValue2));
                                                System.Linq.Expressions.Expression zeroExpression = System.Linq.Expressions.Expression.Constant(0, typeof(int));

                                                if (grandExpression == null)
                                                {
                                                    Expression e1 = Expression.GreaterThanOrEqual(leftCompareExpression, zeroExpression);
                                                    Expression e2 = Expression.LessThanOrEqual(rightCompareExpression, zeroExpression);
                                                    grandExpression = Expression.And(e1, e2);
                                                }
                                                else
                                                {
                                                    Expression e1 = Expression.GreaterThanOrEqual(leftCompareExpression, zeroExpression);
                                                    Expression e2 = Expression.LessThanOrEqual(rightCompareExpression, zeroExpression);
                                                    Expression anded = Expression.And(e1, e2);
                                                    grandExpression = Expression.And(grandExpression, anded);
                                                }

                                            }
                                            else
                                            {
                                                Expression left = Expression.Constant(item.FieldValue, middle.Type);
                                                Expression rigt = Expression.Constant(item.FieldValue2, middle.Type);

                                                if (grandExpression == null)
                                                {
                                                    Expression e1 = Expression.GreaterThanOrEqual(middle, left);
                                                    Expression e2 = Expression.LessThanOrEqual(middle, rigt);
                                                    grandExpression = Expression.And(e1, e2);
                                                }
                                                else
                                                {
                                                    Expression e1 = Expression.GreaterThanOrEqual(middle, left);
                                                    Expression e2 = Expression.LessThanOrEqual(middle, rigt);
                                                    Expression anded = Expression.And(e1, e2);
                                                    grandExpression = Expression.And(grandExpression, anded);
                                                }
                                            }
                                        }
                                        else if (item.FieldValue != null)
                                        {
                                            if (item.IsCustomField)
                                            {
                                                System.Linq.Expressions.Expression left = System.Linq.Expressions.Expression.Property(pe, typeof(T).GetProperty(item.FieldName));
                                                System.Linq.Expressions.Expression right = System.Linq.Expressions.Expression.Constant(item.FieldValue, left.Type);
                                                MethodInfo mi = typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) });

                                                MemberExpression field = Expression.PropertyOrField(pe, item.FieldName);

                                                MethodCallExpression compareExpression = Expression.Call(field, mi, Expression.Constant(item.FieldValue));
                                                System.Linq.Expressions.Expression zeroExpression = System.Linq.Expressions.Expression.Constant(0, typeof(int));

                                                if (grandExpression == null)
                                                {

                                                    grandExpression = System.Linq.Expressions.Expression.GreaterThanOrEqual(compareExpression, zeroExpression);
                                                }
                                                else
                                                {
                                                    System.Linq.Expressions.Expression e1 = System.Linq.Expressions.Expression.GreaterThanOrEqual(compareExpression, zeroExpression);
                                                    grandExpression = System.Linq.Expressions.Expression.And(grandExpression, e1);
                                                }

                                            }
                                            else
                                            {
                                                Expression left = Expression.Property(pe, typeof(T).GetProperty(item.FieldName));
                                                Expression right = Expression.Constant(item.FieldValue, left.Type);
                                                if (grandExpression == null)
                                                {

                                                    grandExpression = Expression.GreaterThanOrEqual(left, right);
                                                }
                                                else
                                                {
                                                    Expression e1 = Expression.GreaterThanOrEqual(left, right);
                                                    grandExpression = Expression.And(grandExpression, e1);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (item.IsCustomField)
                                            {
                                                System.Linq.Expressions.Expression left = System.Linq.Expressions.Expression.Property(pe, typeof(T).GetProperty(item.FieldName));
                                                System.Linq.Expressions.Expression right = System.Linq.Expressions.Expression.Constant(item.FieldValue2, left.Type);
                                                MethodInfo mi = typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) });

                                                MemberExpression field = Expression.PropertyOrField(pe, item.FieldName);

                                                MethodCallExpression compareExpression = Expression.Call(field, mi, Expression.Constant(item.FieldValue2));
                                                System.Linq.Expressions.Expression zeroExpression = System.Linq.Expressions.Expression.Constant(0, typeof(int));

                                                if (grandExpression == null)
                                                {

                                                    grandExpression = System.Linq.Expressions.Expression.GreaterThanOrEqual(compareExpression, zeroExpression);
                                                }
                                                else
                                                {
                                                    System.Linq.Expressions.Expression e1 = System.Linq.Expressions.Expression.GreaterThanOrEqual(compareExpression, zeroExpression);
                                                    grandExpression = System.Linq.Expressions.Expression.And(grandExpression, e1);
                                                }

                                            }
                                            else
                                            {
                                                Expression left = Expression.Property(pe, typeof(T).GetProperty(item.FieldName));
                                                Expression right = Expression.Constant(item.FieldValue2, left.Type);
                                                if (grandExpression == null)
                                                {

                                                    grandExpression = Expression.LessThanOrEqual(left, right);
                                                }
                                                else
                                                {
                                                    Expression e1 = Expression.GreaterThanOrEqual(left, right);
                                                    grandExpression = Expression.And(grandExpression, e1);
                                                }
                                            }
                                        }
                                        break;
                                    }

                                case "NotEqual":
                                    {
                                        Expression left = Expression.Property(pe, typeof(T).GetProperty(item.FieldName));

                                        Expression right = Expression.Constant(item.FieldValue, left.Type);
                                        if (grandExpression == null)
                                        {

                                            grandExpression = Expression.NotEqual(left, right);
                                        }
                                        else
                                        {
                                            Expression e1 = Expression.NotEqual(left, right);
                                            grandExpression = Expression.And(grandExpression, e1);
                                        }

                                        if (item.FieldValue2 != null)
                                        {
                                            right = Expression.Constant(item.FieldValue2, left.Type);
                                            if (grandExpression == null)
                                            {

                                                grandExpression = Expression.NotEqual(left, right);
                                            }
                                            else
                                            {
                                                Expression e1 = Expression.NotEqual(left, right);
                                                grandExpression = Expression.And(grandExpression, e1);
                                            }

                                        }

                                        break;


                                    }

                                case "Exclude":
                                    {
                                        string[] listOfValus = item.FieldValue.ToString().Split(new string[] { ",", "%2C" }, StringSplitOptions.None);
                                        Expression notInListExpression = null;
                                        foreach (string v in listOfValus)
                                        {

                                            MethodInfo mi = typeof(String).GetMethod("Contains", new Type[] { typeof(String) });

                                            MemberExpression field = System.Linq.Expressions.Expression.PropertyOrField(pe, item.FieldName);

                                            MethodCallExpression contains = System.Linq.Expressions.Expression.Call(field, mi, System.Linq.Expressions.Expression.Constant(v));
                                            var doesNotContain = Expression.Not(contains);

                                            if (notInListExpression == null)
                                            {
                                                notInListExpression = doesNotContain;
                                            }
                                            else
                                            {
                                                System.Linq.Expressions.Expression expression = doesNotContain;
                                                notInListExpression = System.Linq.Expressions.Expression.And(notInListExpression, expression);
                                            }




                                        }
                                        if (grandExpression == null)
                                        {

                                            grandExpression = notInListExpression;
                                        }
                                        else
                                        {
                                            System.Linq.Expressions.Expression expression = notInListExpression;
                                            grandExpression = System.Linq.Expressions.Expression.And(grandExpression, expression);
                                        }



                                        break;
                                    }

                                case "IsNotNull":
                                    {
                                        Expression left = Expression.Property(pe, typeof(T).GetProperty(item.FieldName));

                                        Expression right = Expression.Constant(null, left.Type);
                                        if (grandExpression == null)
                                        {

                                            grandExpression = Expression.NotEqual(left, right);
                                        }
                                        else
                                        {
                                            Expression e1 = Expression.NotEqual(left, right);
                                            grandExpression = Expression.And(grandExpression, e1);
                                        }
                                        break;


                                    }
                                case "IsNull":
                                    {
                                        Expression left = Expression.Property(pe, typeof(T).GetProperty(item.FieldName));

                                        Expression right = Expression.Constant(null, left.Type);
                                        if (grandExpression == null)
                                        {

                                            grandExpression = Expression.Equal(left, right);
                                        }
                                        else
                                        {
                                            Expression e1 = Expression.Equal(left, right);
                                            grandExpression = Expression.And(grandExpression, e1);
                                        }
                                        break;


                                    }

                                case "InListInt":
                                    {

                                        string[] listOfValus = item.FieldValue.ToString().Split(',');
                                        List<int> listOfInts = new List<int>();
                                        foreach (string v in listOfValus)
                                        {
                                            int x = Convert.ToInt32(v);
                                            listOfInts.Add(x);
                                        }
                                        Expression inListExpression = null;
                                        foreach (int v in listOfInts)
                                        {
                                            Expression left = Expression.Property(pe, typeof(T).GetProperty(item.FieldName));

                                            Expression right = Expression.Constant(v, left.Type);


                                            Expression equals = Expression.Equal(left, right);

                                            if (inListExpression == null)
                                            {
                                                inListExpression = equals;
                                            }
                                            else
                                            {
                                                System.Linq.Expressions.Expression expression = equals;
                                                inListExpression = System.Linq.Expressions.Expression.Or(inListExpression, expression);
                                            }




                                        }
                                        if (grandExpression == null)
                                        {
                                            grandExpression = inListExpression;
                                        }
                                        else
                                        {
                                            System.Linq.Expressions.Expression expression = inListExpression;
                                            grandExpression = System.Linq.Expressions.Expression.And(grandExpression, expression);
                                        }

                                        break;
                                    }
                                default:
                                    {
                                        Expression left = Expression.Property(pe, typeof(T).GetProperty(item.FieldName));

                                        Expression right = Expression.Constant(item.FieldValue, left.Type);

                                        if (item.FieldValue2 != null && item.FieldValue2.ToString() == "OOORRR")
                                        {
                                            if (grandExpression == null)
                                            {

                                                grandExpression = Expression.Equal(left, right);
                                            }
                                            else
                                            {
                                                Expression e1 = Expression.Equal(left, right);
                                                grandExpression = Expression.Or(grandExpression, e1);
                                            }
                                        }
                                        else if (item.FieldValue2 != null)
                                        {
                                            Expression e1 = Expression.Constant(item.FieldValue2, left.Type);
                                            Expression e2 = Expression.Equal(left, e1);
                                            Expression e3 = Expression.Equal(left, right);

                                            if (grandExpression == null)
                                            {

                                                grandExpression = Expression.Or(e3, e2);
                                            }
                                            else
                                            {
                                                Expression e4 = Expression.Or(e3, e2);
                                                grandExpression = Expression.And(grandExpression, e4);
                                            }
                                        }
                                        else
                                        {
                                            if (grandExpression == null)
                                            {

                                                grandExpression = Expression.Equal(left, right);
                                            }
                                            else
                                            {
                                                Expression e1 = Expression.Equal(left, right);
                                                grandExpression = Expression.And(grandExpression, e1);
                                            }
                                        }

                                        break;
                                    }
                            }
                        }
                        else
                        {
                            Expression left = Expression.Property(pe, typeof(T).GetProperty(item.FieldName));
                            Expression right = Expression.Constant(item.FieldValue, left.Type);
                            if (grandExpression == null)
                            {

                                grandExpression = Expression.Equal(left, right);
                            }
                            else
                            {
                                Expression e1 = Expression.Equal(left, right);
                                grandExpression = Expression.And(grandExpression, e1);
                            }
                        }
                    }
                }

                if (grandExpression == null)
                {
                    return queryableData;
                }
                else
                {
                    MethodCallExpression whereCallExpression = Expression.Call(
                       typeof(Queryable),
                       "Where",
                       new Type[] { queryableData.ElementType },
                       queryableData.Expression,
                       Expression.Lambda<Func<T, bool>>(grandExpression, new ParameterExpression[] { pe }));
                    IQueryable<T> result = queryableData.Provider.CreateQuery<T>(whereCallExpression);
                    return result;
                }
            }
            else
            {
                return queryableData;
            }
        }
        private void GetCustomFieldStringValue(QueryFilterItem filterItem)
        {
            if (filterItem.IsCustomField)
            {
                if (filterItem.FieldValue != null)
                {
                    filterItem.FieldValue = customFilterClass.SetFieldDataType(filterItem.FieldDataType, filterItem.FieldValue);
                    if (filterItem.FieldValue != null && filterItem.FieldValue.ToString().ToLower() == "false")
                        filterItem.FieldValue = null;

                }

                if (filterItem.FieldValue2 != null)
                {
                    filterItem.FieldValue2 = customFilterClass.SetFieldDataType(filterItem.FieldDataType, filterItem.FieldValue2);
                    if (filterItem.FieldValue2 != null && filterItem.FieldValue2.ToString().ToLower() == "false")
                        filterItem.FieldValue2 = null;
                }
            }
        }
    }

}
