using System.ComponentModel.DataAnnotations.Schema;

namespace APIEquipManage.Models
{
    public class Category
    {
        public required int Id { get; set; }
        public int? IdParent { get; set; }
        public required string Name { get; set; }
    }
}
