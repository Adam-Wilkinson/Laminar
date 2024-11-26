using System.Linq.Expressions;
using System.Reflection;

namespace Laminar.Domain.Extensions;

public static class ReflectionHelpers
{
    public static Func<TTarget, TValue> ConstructGetter<TTarget, TValue>(PropertyInfo propertyInfo)
    {
        var parameter = Expression.Parameter(typeof(TTarget));

        Expression accessPropertyValue = Expression.Property(parameter, propertyInfo);
        if (propertyInfo.PropertyType != typeof(TValue)) accessPropertyValue = Expression.Convert(accessPropertyValue, typeof(TValue));

        return Expression.Lambda<Func<TTarget, TValue>>(accessPropertyValue, parameter).Compile();
    }
        
    public static Action<TTarget, TValue> ConstructSetter<TTarget, TValue>(PropertyInfo propertyInfo)
    {
        var instanceParameter = Expression.Parameter(typeof(TTarget));
        var valueParameter = Expression.Parameter(typeof(TValue));

        Expression propertyExpression = Expression.Property(instanceParameter, propertyInfo);
        Expression valueExpression = valueParameter;
        if (propertyInfo.PropertyType != typeof(TValue)) valueExpression = Expression.Convert(valueExpression, propertyInfo.PropertyType);

        var assignExpression = Expression.Assign(propertyExpression, valueExpression);

        return Expression.Lambda<Action<TTarget, TValue>>(assignExpression, instanceParameter, valueParameter).Compile();
    }
}