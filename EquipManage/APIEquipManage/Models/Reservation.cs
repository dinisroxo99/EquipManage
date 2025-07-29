using APIEquipManage.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace APIEquipManage.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int IdEquipment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CanceledAt { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
