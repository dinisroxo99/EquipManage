using APIEquipManage.Models;

namespace APIEquipManage.DTOS
{
    public class CategoryDTO
    {
        public int? Code { get; set; }
        public int? SubCode { get; set; }
        public required string Name { get; set; }
    }
    public class CategoryConflictDTO
    {
        public string Message { get; set; } = "";
        public List<Equipment> Equipamentos { get; set; } = new List<Equipment>();
        public List<Category> Subcategorias { get; set; } = new List<Category>();
    }


}
