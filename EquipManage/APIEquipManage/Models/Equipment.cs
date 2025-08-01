
using APIEquipManage.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;

namespace APIEquipManage.Models
{
    public class Equipment
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Model { get; set; }
        public string? Description {  get; set; }
        public required DateTime CreatedAt { get; set; }
        public required int IdStatus { get; set; }
        public required int IdCategory { get; set; }
        public DateTime? DeletedAt { get; set; }


        [ForeignKey("IdStatus")]
        public StatusOpt? StatusOpt { get; set; }
        [ForeignKey("IdCategory")]
        public  Category? Category { get; set; }
        [ForeignKey("IdEquipment")]
        public List<Image>? Images { get; set; }

    }
}
