namespace APIEquipManage.DTOS
{
    public class UpdateEquipmentDTO
    {
        public required string Name { get; set; }
        public required string Model { get; set; }
        public required string Description { get; set; }
        public required int StatusId { get; set; }
        public required int CategoryId { get; set; }
    }
}
