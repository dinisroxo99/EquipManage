using APIEquipManage.Models;

namespace APIEquipManage.DTOS
{
    public class OptionsDTO
    {
        public required List<string> options { get; set; } = new List<string>();
    }
    public class OptionConflictDTO
    {
        public string Message { get; set; } = "";
        public List<Equipment> Equipamentos { get; set; } = new List<Equipment>();
    }
}
