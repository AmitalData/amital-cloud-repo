using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using System.ComponentModel;

namespace AmitalCloud.Infrastructure.Web.Helpers.TreeFilterQuery
{
    public static class WhereExpression
    {
        public static readonly Type StringType = typeof(string);
        private static readonly Type ExpType = typeof(Expression);
        private static readonly MethodInfo AndExpMethod = ExpType.GetRuntimeMethod(nameof(Expression.AndAlso), new[] { ExpType, ExpType });
        private static readonly MethodInfo OrExpMethod = ExpType.GetRuntimeMethod(nameof(Expression.OrElse), new[] { ExpType, ExpType });
        public static readonly MethodInfo ContainsMethod = StringType.GetRuntimeMethod("Contains", new[] { StringType });
        public static readonly MethodInfo StartsMethod = StringType.GetRuntimeMethod("StartsWith", new[] { StringType });
        public static readonly MethodInfo EndsMethod = StringType.GetRuntimeMethod("EndsWith", new[] { StringType });
        public static readonly MethodInfo CompareToMethod = StringType.GetRuntimeMethod("CompareTo", new[] { StringType });

        private static readonly Type[] AvailableCastTypes =
        {
            typeof(DateTime),
            typeof(DateTime?),
            typeof(DateTimeOffset),
            typeof(DateTimeOffset?),
            typeof(TimeSpan),
            typeof(TimeSpan?),
            typeof(bool),
            typeof(bool?),
            typeof(byte?),
            typeof(sbyte?),
            typeof(short),
            typeof(short?),
            typeof(ushort),
            typeof(ushort?),
            typeof(int),
            typeof(int?),
            typeof(uint),
            typeof(uint?),
            typeof(long),
            typeof(long?),
            typeof(ulong),
            typeof(ulong?),
            typeof(Guid),
            typeof(Guid?),
            typeof(double),
            typeof(double?),
            typeof(float),
            typeof(float?),
            typeof(decimal),
            typeof(decimal?),
            typeof(char),
            typeof(char?),
            typeof(string)
        };

        public static Expression<Func<T, bool>> GetTreeExpression<T>(this QueryFilterItem filter, string suffix = "")
        {
            var expression = Expression.Parameter(typeof(T), "e" + suffix);
            return Expression.Lambda<Func<T, bool>>(GetExpressionForTreeField(expression, filter, suffix), expression);
        }

        private static Expression GetExpressionForTreeField(Expression expression, QueryFilterItem filter, string suffix)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            if (filter.FilterType == "None" || string.IsNullOrEmpty(filter.FilterType) || !string.IsNullOrEmpty(filter.Operator))
                return GetExpressionForField(expression, filter, suffix + "0");

            if (filter.QueryFilterItems == null || !filter.QueryFilterItems.Any())
                throw new ArgumentException("Filter operands with operator type different from TreeFilterType.None cannot be empty.");

            var i = 0;
            var expressions = GetExpressionForTreeField(expression, filter.QueryFilterItems[i], suffix + i);
            var methodInfo = filter.FilterType == "And" ? AndExpMethod : OrExpMethod;
            for (i = 1; i < filter.QueryFilterItems.Count; i++)
            {
                expressions = (BinaryExpression)methodInfo.Invoke(null, new object[] { expressions, GetExpressionForTreeField(expression, filter.QueryFilterItems[i], suffix + i) });
            }
            return expressions;
        }

        private static Expression GetExpressionForField(Expression expression, QueryFilterItem filter, string suffix)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            if (filter.FilterType == "None" || string.IsNullOrWhiteSpace(filter.FieldName))
                throw new ArgumentException("Filter type cannot be None for single filter.");

            return GenerateExpressionOneField(expression, filter);
        }

        private static Expression GenerateExpressionOneField(Expression expression, QueryFilterItem item)
        {
            var op = item.Operator?.Replace("Field", "")?.Trim();

            switch (op)
            {
                case "LessThan": return new LessThan().CreateExpression(expression, item);
                case "LessThanOrEqual": return new LessThanOrEqual().CreateExpression(expression, item);
                case "GreaterThanOrEqual": return new GreaterThanOrEqual().CreateExpression(expression, item);
                case "LargerThan":
                case "GreaterThan":
                    return new GreaterThan().CreateExpression(expression, item);
                case "Contains": return new Contains().CreateExpression(expression, item);
                case "NotContains": return new NotContains().CreateExpression(expression, item);
                case "Equal": return new Equal().CreateExpression(expression, item);
                case "NotEqual": return new NotEqual().CreateExpression(expression, item);
                case "IsEmpty":
                case "IsNull": return new IsEmpty().CreateExpression(expression, item);
                case "IsNotEmpty":
                case "IsNotNull": return new IsNotEmpty().CreateExpression(expression, item);
                case "PartnerEntity": return new PartnerEntity().CreateExpression(expression, item);
                case "StartsWith": return new StartsWith().CreateExpression(expression, item);
                case "EndsWith": return new EndsWith().CreateExpression(expression, item);
                case "InList": return new InList().CreateExpression(expression, item);
                case "InListExact": return new InListExact().CreateExpression(expression, item);
                case "Exclude": return new Exclude().CreateExpression(expression, item);
                case "InListInt": return new InListInt().CreateExpression(expression, item);
                default: throw new InvalidOperationException("Operator not supported.");
            }

        }

        public static object TryCastFieldValueType(object value, Type type)
        {
            if (value == null || (!AvailableCastTypes.Contains(type) && !type.GetTypeInfo().IsEnum))
                throw new InvalidCastException($"Cannot convert value to type {type.Name}."); 
            if (value.GetType() == type) return value; 

            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            try
            {
                if (underlyingType.GetTypeInfo().IsEnum) { return Enum.Parse(underlyingType, value.ToString()); }
                var converter = TypeDescriptor.GetConverter(underlyingType); 
                if (converter != null && converter.IsValid(value)) { return converter.ConvertFromInvariantString(value.ToString()); }
                throw new InvalidCastException($"Cannot convert value to type {type.Name }.");
            }
            catch
            {
                throw new InvalidCastException($"Cannot convert value to type {type.Name}.");
            }
        }

        public static Expression ToStaticParameterExpressionOfType(object obj, Type type)
            => Expression.Convert(
                Expression.Property(
                    Expression.Constant(new { obj }),
                    "obj"),
                type);

        public static PropertyInfo GetDeclaringProperty(Expression expression, string name)
        {
            var type = expression.Type;
            var property = type.GetRuntimeProperties().SingleOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (property == null) throw new InvalidOperationException(string.Format("Property '{0}' not found on type '{1}'", name, type));

            if (name == "PartnerEntityField") return type.GetRuntimeProperties().FirstOrDefault();

            if (type != property.DeclaringType)
            {
                property = property.DeclaringType.GetRuntimeProperties().SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
            return property;
        }
    }
}
