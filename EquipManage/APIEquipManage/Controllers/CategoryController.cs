using APIEquipManage.Data;
using APIEquipManage.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIEquipManage.Models;
using System;
using Microsoft.IdentityModel.Tokens;
using APIEquipManage.Extensions;

namespace APIEquipManage.Controllers
{
    [ApiController]
    [Route("api/category")]
    public class CategoryController(EquipManageContext equipManageContext) : ControllerBase
    {

        private readonly EquipManageContext _equipManageContext = equipManageContext;

        [HttpGet]
        public async Task<IActionResult> GetCategorys()
        {
            try
            {
                var categories = await _equipManageContext.Category.AsNoTracking().ToListAsync();
                if (categories.Count() < 1)
                {
                    return NoContent();
                }
                var response = new List<CategoryDTO>();
                foreach (var category in categories)
                {
                    response.Add(new CategoryDTO()
                    {
                        Code = category.Id,
                        SubCode = category.IdParent,
                        Name = category.Name
                    });
                }

                return Ok(response);

            }
            catch (Exception e)
            {

                return BadRequest(e);
            }
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> NewSubCategory([FromRoute] int id, [FromBody] List<CategoryDTO> listSubCategories)
        {
            try
            {
                var category = await _equipManageContext.Category.FindAsync(id);
                if (category == null) { return NotFound(); }

                if (listSubCategories.Count < 1) { return BadRequest("No items found!"); }
                var newSubCategories = listSubCategories.Select(sub => new Category() { Name = sub.Name, IdParent = id }).ToList();

                await _equipManageContext.Category.AddRangeAsync(newSubCategories);
                await _equipManageContext.SaveChangesAsync();

                var response = new
                {
                    Message = "The categories have been added successfully",
                    NewCategories = newSubCategories.Select(sub => new CategoryDTO() { Name = sub.Name }).ToList()
                };
                return Ok(response);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> NewCategory([FromBody] List<CategoryDTO> categories)
        {
            if (categories.Count < 1)
            {
                return BadRequest("No items found!");
            }
            try
            {
                var newCategories = categories.Select(cat => new Category() { Name = cat.Name });

                await _equipManageContext.Category.AddRangeAsync(newCategories);
                await _equipManageContext.SaveChangesAsync();

                var response = new
                {
                    Message = "The categories have been added successfully",
                    NewCategories = newCategories.Select(cat => new CategoryDTO() { Name = cat.Name }).ToList(),
                };
                return Ok(response);
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
                if (category == null) { return BadRequest("Cannot delete category because there are associated equipment or subcategories."); }
                var equipmentdependence = await _equipManageContext.Equipment.Where(x => x.IdCategory == id).ToListAsync();
                var categorydependence = await _equipManageContext.Category.Where(x => x.IdParent == category.Id).ToListAsync();
                if (equipmentdependence.Count > 0 || categorydependence.Count > 0)
                {
                    return Conflict(new CategoryConflictDTO
                    {
                        Message = $"Cannot delete this category \"{category.Name}\", because there are dependencie(s).",
                        Equipamentos = equipmentdependence,
                        Subcategorias = categorydependence
                    });
                }
                _equipManageContext.Category.Remove(category);
                await _equipManageContext.SaveChangesAsync();
                return Ok($"The category \"{category.Name}\" was deleted successfully.");


            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
    }
}