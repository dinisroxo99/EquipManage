using APIEquipManage.Models;

namespace APIEquipManage.Models
{
    public class Image
    {
        public int Id { get; set; }
        public int IdEquipment { get; set; }
        public string Path { get; set; }
    }
}