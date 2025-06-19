using AmitalCloud.Infrastructure.Domain.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace AmitalCloud.Infrastructure.Web.Helpers.TreeFilterQuery
{
    public abstract class OperatorExpression
    {
        public Expression LeftExpression { get; set; }
        public Expression RightExpression { get; set; }
        public Expression BaseExpression { get; set; }
        public QueryFilterItem QueryFilterItem { get; set; }
        public Expression CreateExpression(Expression expression, QueryFilterItem queryFilterItem)
        {
            BaseExpression = expression;
            QueryFilterItem = queryFilterItem;
            if (!queryFilterItem.IsCustomField) SetLeftRightExpressions();
            else SetCustomFieldLeftRightExpressions();
            return Build();
        }

        private void SetLeftRightExpressions()
        {
            LeftExpression = Expression.Property(BaseExpression, WhereExpression.GetDeclaringProperty(BaseExpression, QueryFilterItem.FieldName));
            RightExpression = GetRightExpression();
        }
        private void SetCustomFieldLeftRightExpressions()
        {
            if (QueryFilterItem.Operator == "IsEmpty" || QueryFilterItem.Operator == "IsNotEmpty")
            {
                QueryFilterItem.FieldValue = null;
                LeftExpression = Expression.Property(BaseExpression, WhereExpression.GetDeclaringProperty(BaseExpression, QueryFilterItem.FieldName));
                RightExpression = GetRightExpression();
            }
            else
            {
                MethodInfo methodInfo = typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) });
                RightExpression = Expression.Constant(0, typeof(int));
                if (QueryFilterItem.FieldValue == null || string.IsNullOrEmpty(QueryFilterItem.FieldValue.ToString())) return;

                if (IsValueTypeField())
                {
                    LeftExpression = Expression.Call(Expression.PropertyOrField(BaseExpression, QueryFilterItem.FieldName), methodInfo, Expression.PropertyOrField(BaseExpression, GetFieldName(QueryFilterItem.FieldValue)));
                }
                else
                {
                    LeftExpression = Expression.Call(Expression.PropertyOrField(BaseExpression, QueryFilterItem.FieldName), methodInfo, Expression.Constant(QueryFilterItem.FieldValue));
                }
            }
        }
        private Expression GetRightExpression()
        {
            if (QueryFilterItem.FieldValue == null || string.IsNullOrEmpty(QueryFilterItem.FieldValue.ToString())) return null;

            if (IsValueTypeField())
            {
                return Expression.Property(BaseExpression, WhereExpression.GetDeclaringProperty(BaseExpression, GetFieldName(QueryFilterItem.FieldValue)));
            }
            return WhereExpression.ToStaticParameterExpressionOfType(WhereExpression.TryCastFieldValueType(QueryFilterItem.FieldValue, LeftExpression.Type), LeftExpression.Type);
        }

        public abstract Expression Build();
        private string GetFieldName(object name)
        {
            if (name == null || string.IsNullOrEmpty(name.ToString())) return null;
            var names = name.ToString().Split('.');
            return names[names.Length - 1];
        }
        public bool IsValueTypeField() => QueryFilterItem.Operator?.Contains("Field") == true;
    }
    public class GreaterThan : OperatorExpression
    {
        public override Expression Build() => Expression.GreaterThan(LeftExpression, RightExpression);
    }

    public class LessThan : OperatorExpression
    {
        public override Expression Build() => Expression.LessThan(LeftExpression, RightExpression);
    }

    public class GreaterThanOrEqual : OperatorExpression
    {
        public override Expression Build() => Expression.GreaterThanOrEqual(LeftExpression, RightExpression);
    }

    public class LessThanOrEqual : OperatorExpression
    {
        public override Expression Build() => Expression.LessThanOrEqual(LeftExpression, RightExpression);
    }

    public class Equal : OperatorExpression
    {
        public override Expression Build()
        {
            return Expression.Equal(LeftExpression, RightExpression);
        }
    }

    public class NotEqual : OperatorExpression
    {
        public override Expression Build()
        {
            return Expression.NotEqual(LeftExpression, RightExpression);
        }
    }

    public class Contains : OperatorExpression
    {
        public override Expression Build()
        {
            if (IsValueTypeField())
            {
                return Expression.Call(LeftExpression, WhereExpression.ContainsMethod, RightExpression);
            }

            MemberExpression field = Expression.PropertyOrField(BaseExpression, QueryFilterItem.FieldName);
            return Expression.Call(field, WhereExpression.ContainsMethod, Expression.Constant(QueryFilterItem.FieldValue, WhereExpression.StringType));

        }
    }

    public class NotContains : OperatorExpression
    {
        public override Expression Build()
        {
            if (IsValueTypeField())
            {
                return Expression.Not(Expression.Call(LeftExpression, WhereExpression.ContainsMethod, RightExpression));
            }

            MemberExpression field = Expression.PropertyOrField(BaseExpression, QueryFilterItem.FieldName);
            return Expression.Not(Expression.Call(field, WhereExpression.ContainsMethod, Expression.Constant(QueryFilterItem.FieldValue, WhereExpression.StringType)));
        }
    }

    public class IsEmpty : OperatorExpression
    {
        public override Expression Build() => Expression.Equal(LeftExpression, WhereExpression.ToStaticParameterExpressionOfType(null, LeftExpression.Type));
    }

    public class IsNotEmpty : OperatorExpression
    {
        public override Expression Build() => Expression.NotEqual(LeftExpression, WhereExpression.ToStaticParameterExpressionOfType(null, LeftExpression.Type));
    }
    public class StartsWith : OperatorExpression
    {
        public override Expression Build()
        {
            MemberExpression field = Expression.PropertyOrField(BaseExpression, QueryFilterItem.FieldName);
            return Expression.Call(field, WhereExpression.StartsMethod, Expression.Constant(QueryFilterItem.FieldValue ?? ""));
        }
    }
    public class EndsWith : OperatorExpression
    {
        public override Expression Build()
        {
            MemberExpression field = Expression.PropertyOrField(BaseExpression, QueryFilterItem.FieldName);
            return Expression.Call(field, WhereExpression.EndsMethod, Expression.Constant(QueryFilterItem.FieldValue ?? ""));
        }
    }
    public class InList : OperatorExpression
    {
        public override Expression Build()
        {
            Expression expression = null;
            var values = QueryFilterItem.FieldValue?.ToString()?.Split(',') ?? Array.Empty<string>();
            foreach (string value in values)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    expression = AddValueExpression(expression, value);
                }
            }
            return expression;
        }

        private Expression AddValueExpression(Expression expression, string value)
        {
            MethodCallExpression contains = Expression.Call(Expression.PropertyOrField(BaseExpression, QueryFilterItem.FieldName), WhereExpression.ContainsMethod, Expression.Constant(value));
            if (expression == null) return contains;
            return Expression.Or(expression, contains); ;
        }
    }

    public class InListExact : OperatorExpression
    {
        public override Expression Build()
        {
            Expression expression = null;
            var values = QueryFilterItem.FieldValue?.ToString()?.Split(',') ?? Array.Empty<string>();
            foreach (string value in values)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    expression = AddValueExpression(expression, value);
                }
            }
            return expression;
        }

        private Expression AddValueExpression(Expression expression, string value)
        {
            Expression right = (value != null && value.ToLower() == "null") ? Expression.Constant(null) : Expression.Constant(value);
            expression = expression == null ? Expression.Equal(LeftExpression, right) : Expression.Or(expression, Expression.Equal(LeftExpression, right));
            return expression;
        }
    }

    public class Exclude : OperatorExpression
    {
        public override Expression Build()
        {
            Expression expression = null;
            var values = QueryFilterItem.FieldValue?.ToString()?.Split(new string[] { ",", "%2C" }, StringSplitOptions.None) ?? Array.Empty<string>(); ;
            foreach (string value in values)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    expression = AddValueExpression(expression, value);
                }
            }
            return expression;
        }

        private Expression AddValueExpression(Expression expression, string value)
        {
            MemberExpression field = Expression.PropertyOrField(expression, QueryFilterItem.FieldName);
            MethodCallExpression contains = Expression.Call(field, WhereExpression.ContainsMethod, Expression.Constant(value));
            if (expression == null) return Expression.Not(contains);
            return Expression.And(expression, Expression.Not(contains)); ;
        }
    }

    public class InListInt : OperatorExpression
    {
        public override Expression Build()
        {
            List<int> listOfInts = new List<int>();
            Expression expression = null;
            var values = QueryFilterItem.FieldValue?.ToString()?.Split(',') ?? Array.Empty<string>();
            foreach (string value in values)
            {
                listOfInts.Add(Convert.ToInt32(value));
            }

            foreach (int value in listOfInts)
            {
                expression = AddValueExpression(expression, value);

            }
            return expression;
        }

        private Expression AddValueExpression(Expression expression, int value)
        {
            Expression right = Expression.Constant(value, LeftExpression.Type);
            Expression equals = Expression.Equal(LeftExpression, right);
            if (expression == null) return equals;
            return Expression.Or(expression, equals);
        }
    }
}
