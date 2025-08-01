using System.ComponentModel.DataAnnotations;

namespace APIEquipManage.DTOS
{
    public class ImageUploadDto
    {
        public required List<IFormFile> Images { get; set; }
    }
}
