using APIEquipManage.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIEquipManage.Models
{
    public class Image
    {
        public int Id { get; set; }
        public required int IdEquipment { get; set; }
        public required string ImagePath { get; set; }

        [ForeignKey("IdEquipment")]
        public  Equipment? Equipment { get; set; }
    }
}