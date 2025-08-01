using APIEquipManage.Data;
using APIEquipManage.DTOS;
using APIEquipManage.Extensions;
using APIEquipManage.Handlers;
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
    public async Task<IActionResult> NewImage([FromRoute] int id, [FromForm] ImageUploadDto files)
    {
        
        if (files.Images.Count < 1)
        {
            return BadRequest("No files provided.");
        }
        try
        {
            
            var equipment = await _equipManageContext.Equipment.FindAsync(id);
            if (equipment == null) { return NotFound(); }
            var addedImg = new List<Image>();
            foreach (var image in files.Images)
            {
                string imagePath = new ImageHandler().UploadImages(image);

                if (imagePath != null) 
                {
                    var newImg = new Image() { IdEquipment = equipment.Id, Path = imagePath };
                    await _equipManageContext.Image.AddAsync(newImg); 
                    addedImg.Add(newImg);   
                }
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
                var x = new ImageHandler().DeleteImage(img.Path);
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

