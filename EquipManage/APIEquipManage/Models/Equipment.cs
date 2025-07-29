
using APIEquipManage.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public required int StatusId { get; set; }
        public required int CategoryId { get; set; }


        
        
        public required Status Status { get; set; }
        public required Category Category { get; set; }
        public List<Image>? Images { get; set; }

    }
}
