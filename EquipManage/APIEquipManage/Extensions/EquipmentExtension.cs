using Microsoft.EntityFrameworkCore;
using APIEquipManage.Models;

namespace APIEquipManage.Extensions
{
    public static class EquipmentExtension
    {
        public static IQueryable<Equipment> IncludeAll(this IQueryable<Equipment> query)
        {
            return query
                .Include(e => e.Category)
                .Include(e => e.Status)
                .Include(e => e.Images);
        }

        public static IQueryable<Equipment> IncludeImg(this IQueryable<Equipment> query)
        {
            return query.Include(e => e.Images);
        }
    }
}
