using APIEquipManage.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIEquipManage.Models
{
    public class Image
    {
        public int Id { get; set; }

        [ForeignKey("IdEquipment")]
        public required int IdEquipment { get; set; }
        public required string Path { get; set; }

       
    }
}