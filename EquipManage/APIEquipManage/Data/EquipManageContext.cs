using Microsoft.EntityFrameworkCore;
using APIEquipManage.Models;


namespace APIEquipManage.Data
{
    public class EquipManageContext : DbContext
    {
        public EquipManageContext(DbContextOptions<EquipManageContext> options) : base(options) { }

        public DbSet<Equipment> Equips { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Status> Statuss { get; set; }
        public DbSet<StatusOpt> StatusOpts { get; set; }   

    }
}
