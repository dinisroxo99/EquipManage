using APIEquipManage.Models;
using System.Text.Json.Serialization;

namespace APIEquipManage.DTOS
{
    public class UpdateEquipmentDTO
    {
        public  string Name { get; set; }
        public  string Model { get; set; }
        public  string Description { get; set; }
        public  int StatusId { get; set; }
        public  int CategoryId { get; set; }
    }

    public class NewEquipmentDTO
    {
        public required string Name { get; set; }
        public required string Model { get; set; }
        public string? Description { get; set; }
        public required int StatusId { get; set; }
        public required int CategoryId { get; set; }
    }

    public class GetEquipmentDTO
    {
        public int Code { get; set; }
        public required string Name { get; set; }
        public required string Model { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Status { get; set; }
    }

    public class GetDetailedEquipmentDTO
    {
        public int Code { get; set; }
        public required string Name { get; set; }
        public required string Model { get; set; }
        public required string Description { get; set; }
        public required string Status { get; set; }
        public required string Category { get; set; }
    }

    public class DeletedEquipmentDTO
    {
        public int Code { get; set; }
        public required string Name { get; set; }
        public required string Model { get; set; }
        public required string Status { get; set; }
        public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
        public List<Reservation> CanceledReservation = new List<Reservation>();
    }
}
