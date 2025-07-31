using APIEquipManage.Data;
using APIEquipManage.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIEquipManage.Models;
using System;
using Microsoft.IdentityModel.Tokens;
using APIEquipManage.Extensions;



[ApiController]
[Route("api/category")]
public class CategoryController : ControllerBase
{

    private readonly EquipManageContext _equipManageContext;

    public CategoryController(EquipManageContext equipManageContext)
    {
        _equipManageContext = equipManageContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategorys()
    {
        try
        {
            var category = await _equipManageContext.Category.AsNoTracking().ToListAsync();
            if (category.Count() < 1)
            {
                return NoContent();
            }
            return Ok(category);

        }
        catch (Exception e)
        {

            return BadRequest(e.Message);
        }
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> NewSubCategory([FromRoute]int id, [FromBody] SubCategoryDTO subCategory)
    {
        try
        {
            var category = await _equipManageContext.Category.FindAsync(id);
            var ListSubCategorys = new List<Category>();
            foreach (var item in subCategory.SubCategory)
            {
                var newSubCategory = new Category { IdParent = id, Name = item.ToString() };
                await _equipManageContext.AddAsync(newSubCategory);
                ListSubCategorys.Add(newSubCategory);
                
            }
            if (subCategory.SubCategory.Count < 1)
            {
                return NoContent();
            }
            await _equipManageContext.SaveChangesAsync();
            return Ok(ListSubCategorys);
        }
        catch (Exception e)
        {

            return BadRequest(e.Message);
        }
    }
    [HttpPost]
    public async Task<IActionResult> NewCategory([FromBody] CategoryDTO category)
    {
        if (category.Category.Count < 1)
        {
            return NoContent();
        }
        try
        {
            var ListCategorys = new List<Category>();
            foreach (var name in category.Category)
            {
                var newCategory = new Category { Name = name.ToString() };
                ListCategorys.Add(newCategory);
                await _equipManageContext.Category.AddAsync(newCategory);
            }
            await _equipManageContext.SaveChangesAsync();
            return Ok(ListCategorys);
        }
        catch (Exception e)
        {

            return BadRequest(e.Message);
        }
        
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] int id)
    {
        try
        {
            var category = await _equipManageContext.Category.FindAsync(id);
            if (category == null) { return NotFound(); }
            var equipmentdependence = await _equipManageContext.Equipment.Where(x => x.CategoryId == id).ToListAsync();
            var categorydependence = await _equipManageContext.Category.Where(x => x.IdParent == id).ToListAsync();
            if (equipmentdependence.Count > 0 || categorydependence.Count > 0)
            {
                return Conflict(new CategoryConflictDTO
                {
                    Message = "Cannot delete category because there are dependencies.",
                    Equipamentos = equipmentdependence,
                    Subcategorias = categorydependence
                });
            }
            _equipManageContext.Category.Remove(category);
            await _equipManageContext.SaveChangesAsync();
            return Ok(category);

        }
        catch (Exception e)
        {

            return BadRequest(e.Message);
        }
    }
}

