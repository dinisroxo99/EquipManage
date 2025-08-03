using APIEquipManage.Models;

namespace APIEquipManage.DTOS
{
    public class GetReservationDTO
    {
        public required int Code { get; set; }
        public required GetEquipmentDTO Equipment { get; set; }
        public required DateTime Start { get; set; }
        public required DateTime End { get; set; }
    }

    public class GetDetailedReservationDTO
    {
        public required GetDetailedEquipmentDTO Equipment { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime Start { get; set; }
        public required DateTime End { get; set; }
    }

}
