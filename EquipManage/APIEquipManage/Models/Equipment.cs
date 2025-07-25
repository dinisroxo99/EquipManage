
using APIEquipManage.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.NetworkInformation;

namespace APIEquipManage.Models
{
    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Description {  get; set; }
        public DateTime CreatedAt { get; set; }
        public int IdStatus { get; set; }
        public int IdCategory { get; set; }

    }
}
