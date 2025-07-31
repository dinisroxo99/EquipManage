using APIEquipManage.Data;
using APIEquipManage.DTOS;
using APIEquipManage.Extensions;
using APIEquipManage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/equioment/{id}")]
public class ImageController : ControllerBase
{
    private readonly EquipManageContext _equipManageContext;

    public ImageController(EquipManageContext equipManageContext)
    {
        _equipManageContext = equipManageContext;
    }

    [HttpPost("image")]
    public async Task<IActionResult> NewImage([FromRoute] int id, [FromBody] UpdateImagesDTO updateImages)
    {
        try
        {
            var equipment = await _equipManageContext.Equipment.IncludeImg().FirstAsync(x => x.Id == id);
            var addedImg = new List<Image>();
            foreach (var imgPath in updateImages.ImageUrl)
            {
                var img = new Image { ImagePath = imgPath , IdEquipment = id};
                await _equipManageContext.Image.AddAsync(img);
                addedImg.Add(img);
            }
            await _equipManageContext.SaveChangesAsync();
            return Ok(addedImg);
        }
        catch (Exception e)
        {

            return BadRequest(e.Message);
        }
    }
    [HttpDelete("image/{idImg}")]
    public async Task<IActionResult> DeleteImage([FromRoute] int id, [FromRoute] int idImg)
    {
        try
        {
            var img = await _equipManageContext.Image.FindAsync(idImg);
            if (img != null && img.IdEquipment == id)
            {
                _equipManageContext.Image.Remove(img);
                await _equipManageContext.SaveChangesAsync();
                return Ok(img);
            }
            return NotFound();
                
        }
        catch (Exception e)
        {

            return BadRequest(e.Message);
        }
    }


}

