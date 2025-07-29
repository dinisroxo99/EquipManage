using APIEquipManage.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace APIEquipManage.Models
{
    public class Status
    {
        public required int Id { get; set; }
        public required int IdOptions { get; set; }
        public required DateTime CreatedAt { get; set; }

        public required StatusOpt StatusOpt { get; set; }
    }

}
