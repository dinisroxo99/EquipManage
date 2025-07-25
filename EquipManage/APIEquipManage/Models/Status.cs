using APIEquipManage.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace APIEquipManage.Models
{
    public class Status
    {
        public int Id { get; set; }
        public int IdOptions { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
