using APIEquipManage.Models;

namespace APIEquipManage.Models
{
    public class Image
    {
        public int Id { get; set; }
        public required int IdEquipment { get; set; }
        public required string ImagePath { get; set; }

        public  Equipment? Equipment { get; set; }
    }
}