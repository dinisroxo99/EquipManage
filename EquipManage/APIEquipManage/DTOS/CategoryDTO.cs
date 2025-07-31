using APIEquipManage.Models;

namespace APIEquipManage.DTOS
{
    public class SubCategoryDTO
    {
        public required List<string> SubCategory {  get; set; } = new List<string>();
    }

    public class CategoryDTO
    {
        public required List<string> Category { get; set; } = new List<string>();
    }
    public class CategoryConflictDTO
    {
        public string Message { get; set; } = "";
        public List<Equipment> Equipamentos { get; set; } = new List<Equipment>();
        public List<Category> Subcategorias { get; set; } = new List<Category>();
    }


}
