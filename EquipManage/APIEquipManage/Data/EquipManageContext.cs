using Microsoft.EntityFrameworkCore;
using APIEquipManage.Models;


namespace APIEquipManage.Data
{
    public class EquipManageContext : DbContext
    {
        public EquipManageContext(DbContextOptions<EquipManageContext> options) : base(options) { }
        public DbSet<Category> Category { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<Image> Image { get; set; }
        public DbSet<Reservation> Reservation { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<StatusOpt> StatusOpt { get; set; }   

    }
}
