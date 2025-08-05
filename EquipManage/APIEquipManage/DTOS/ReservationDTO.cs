using APIEquipManage.Models;

namespace APIEquipManage.DTOS
{
    public class ReservationDTO
    {
        public required int Code { get; set; }
        public required EquipmentDTO Equipment { get; set; }
        public required DateTime Start { get; set; }
        public required DateTime End { get; set; }
        public DateTime? CanceledAt { get; set; } 
    }

    public class DetailedReservationDTO
    {
        public required DetailedEquipmentDTO Equipment { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime Start { get; set; }
        public required DateTime End { get; set; }
    }

    public class NewReservationDTO
    {
        public required int EquipmentId { get; set; }
        public required DateTime Start { get; set; }
        public required DateTime End { get; set; }
    }
}
