using System.Linq.Expressions;

namespace APIEquipManage.Extensions
{
    public static class OrderByExtension
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string field, string order)
        {
            if (string.IsNullOrWhiteSpace(field)) return source;

            var param = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(param, field);
            var lambda = Expression.Lambda(property, param);

            string methodName = order.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";

            var result = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.Type)
                .Invoke(null, new object[] { source, lambda });

            return (IQueryable<T>)result!;
        }
    }
}
