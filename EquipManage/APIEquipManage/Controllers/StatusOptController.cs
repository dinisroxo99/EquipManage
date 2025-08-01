

using APIEquipManage.Data;
using APIEquipManage.DTOS;
using APIEquipManage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/status/options")]
public class StatusOptController : ControllerBase
{
    private readonly EquipManageContext _equipManageContext;

    public StatusOptController(EquipManageContext equipManageContext)
    {
        _equipManageContext = equipManageContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetOptions()
    {
        try
        {
            var options = await _equipManageContext.StatusOpt.AsNoTracking().ToListAsync();
            if (options.Count < 1)
            {
                return NoContent();
            }
            return Ok(options);
        }
        catch (Exception e)
        {

            return BadRequest(e.Message);
        }
    }
    [HttpPost]
    public async Task<IActionResult> NewOption([FromBody] OptionsDTO options)
    {
        if (options == null) { return BadRequest(); }
        try
        {
            var optionsList = new List<StatusOpt>();
            foreach (var item in options.options)
            {
                var newOption = new StatusOpt { Name = item.ToString() };
                optionsList.Add(newOption);
                await _equipManageContext.StatusOpt.AddAsync(newOption);
            }
            await _equipManageContext.SaveChangesAsync();
            return Ok(optionsList);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOption([FromRoute] int id)
    {
        try
        {
            var option = await _equipManageContext.StatusOpt.FindAsync(id);
            if (option == null){ return Forbid(); }
            var optionDependence = await _equipManageContext.Equipment.Where(x => x.StatusId == id).ToListAsync();
            if (optionDependence.Count() > 0) {
                return Conflict(new OptionConflictDTO
                {
                    Message = "Cannot delete category because there are dependencies.",
                    Equipamentos = optionDependence
                });
            }
            if (option.Name == "Deleted" || option.Name == "Avalable")
            {
                return Forbid();
            }
            _equipManageContext.StatusOpt.Remove(option);
            await _equipManageContext.SaveChangesAsync();
            return Ok(option);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


}

