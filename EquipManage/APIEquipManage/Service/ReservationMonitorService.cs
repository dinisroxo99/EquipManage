using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using APIEquipManage.Data;

namespace APIEquipManage.Service
{
    public class ReservationMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        
        public ReservationMonitorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetService<EquipManageContext>();

                var now = DateTime.UtcNow;

                var activateReservations = await dbContext.Reservation.Where(r => r.StartDate <= now && r.EndDate > now).ToListAsync(stoppingToken);
                
                if (activateReservations.Count == 0){ return; }

                foreach (var reservation in activateReservations)
                {
                    var equipment = await dbContext.Equipment.FindAsync(new object[] { reservation.IdEquipment }, stoppingToken);
                    if (equipment != null && equipment.StatusId != 2) { equipment.StatusId = 2; }// 2 --> is "not avaliable"
                }

                await dbContext.SaveChangesAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
