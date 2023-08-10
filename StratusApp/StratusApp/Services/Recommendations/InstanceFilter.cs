using Utils.DTO;
using System.Linq.Expressions;
using System.Reflection;

namespace StratusApp.Services.Recommendations
{
    public class InstanceFilter
    {
        private readonly HashSet<FilterField> _filters = new HashSet<FilterField>() { FilterField.Price, FilterField.Memory, FilterField.VCPU};
        private readonly Dictionary<FilterField, Func<Expression, Expression, BinaryExpression>> _filterMethods = new Dictionary<FilterField, Func<Expression, Expression, BinaryExpression>>()
        {
            [FilterField.Price] = Expression.LessThanOrEqual,
            [FilterField.VCPU] = Expression.GreaterThanOrEqual,
            [FilterField.Memory] = Expression.LessThanOrEqual,
            [FilterField.Storage] = Expression.GreaterThanOrEqual,
        };
        private readonly Dictionary<FilterField, Func<InstanceDetails, object>> _filterValues = new Dictionary<FilterField, Func<InstanceDetails, object>>();

        public void AddFilter(FilterField field)
        {
            _filters.Add(field);
        }

        public void AddFilterValue(FilterField field, Func<InstanceDetails, object> method)
        {
            _filterValues.Add(field,method);
        }

        public Func<AlternativeInstance, bool> Filter(InstanceDetails instance)
        {
            Expression filterExpression = Expression.Constant(true);
            var alternativeInstanceParameter = Expression.Parameter(typeof(AlternativeInstance), "alternativeInstance");
       
            foreach (var filterField in _filters)
            {
                string path = string.Empty;
                var filterValue = GetPropertyInfo(typeof(InstanceDetails), filterField.ToString(), instance, ref path);
                path = path.Substring(1);

                if (filterValue == null)
                {
                    continue;
                }

                if(_filterValues.ContainsKey(filterField))
                {
                    filterValue = _filterValues[filterField].Invoke(instance);
                }

                Expression prop = path.Split('.').Aggregate((Expression)alternativeInstanceParameter, (exp, n) => Expression.Property(exp, n));
 
                var filterExpressionForField = _filterMethods[filterField](prop, Expression.Constant(filterValue));
                
                filterExpression = Expression.And(filterExpression, filterExpressionForField);
            }

            return Expression.Lambda<Func<AlternativeInstance, bool>>(filterExpression, alternativeInstanceParameter).Compile();
        }

        private object? GetPropertyInfo(Type type, string propertyName, object value, ref string? path)
        {
            if (type == typeof(object)) return null;

            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property.Name == propertyName)
                {
                    path += "." + property.Name;

                    foreach (PropertyInfo prop in property.PropertyType.GetProperties())
                    {
                        if(prop.Name == "Value" && value != null)
                        {
                            path += "." + prop.Name;
                            value = property.GetValue(value);

                            return value != null ? prop.GetValue(value) : null;
                        }
                    }

                    return value != null ? property.GetValue(value) : null;
                }

                if (property.PropertyType.IsClass && value != null)
                {
                    path += "." + property.Name;
                    var res = GetPropertyInfo(property.PropertyType, propertyName, property.GetValue(value), ref path);

                    if (res != null) return res;

                    path = path.Substring(0, path.LastIndexOf('.'));
                }
            }

            return null;
        }
    }
}

