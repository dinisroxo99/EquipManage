using APIEquipManage.Models;

namespace APIEquipManage.DTOS
{
    public class OptionsDTO
    {
        public int? Code { get; set; }
        public required string Name { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
    public class OptionConflictDTO
    {
        public string? Message { get; set; } = "";
        public required Equipment Equipment { get; set; }
    }
}
