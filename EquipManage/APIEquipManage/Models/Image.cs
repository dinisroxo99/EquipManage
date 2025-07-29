using APIEquipManage.Models;

namespace APIEquipManage.Models
{
    public class Image
    {
        public required int Id { get; set; }
        public required int IdEquipment { get; set; }
        public required string Path { get; set; }

        public required Equipment Equipment { get; set; }
    }
}