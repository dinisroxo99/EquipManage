using APIEquipManage.Data;
using APIEquipManage.DTOS;
using APIEquipManage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIEquipManage.Controllers
{
    [ApiController]
    [Route("api/status/options")]
    public class StatusOptController(EquipManageContext equipManageContext) : ControllerBase
    {
        private readonly EquipManageContext _equipManageContext = equipManageContext;
        private static readonly List<string> _protectedSattus = ["Available", "Deleted"];
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
                var response = options.Select(opt => new OptionsDTO() { Name = opt.Name });
                return Ok(response);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> NewOption([FromBody] List<OptionsDTO> options)
        {
            if (options == null) { return BadRequest(); }
            try
            {
                var newOptions = options.Select(opt => new StatusOpt() { Name = opt.Name, CreatedAt = DateTime.UtcNow });
                await _equipManageContext.StatusOpt.AddRangeAsync(newOptions);
                await _equipManageContext.SaveChangesAsync();

                var response = newOptions.Select(opt => new OptionsDTO() { Code = opt.Id, Name = opt.Name, CreatedAt = opt.CreatedAt });
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException?.Message ?? e.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOption([FromRoute] int id)
        {
            try
            {
                var option = await _equipManageContext.StatusOpt.FindAsync(id);
                if (option == null) { return NotFound(); }
                if (_protectedSattus.Contains(option.Name))
                {
                    return BadRequest("It is not possible to delete \"Deleted\" or \"Avaliable\"");
                }

                var optionDependence = await _equipManageContext.Equipment.Where(x => x.IdStatus == id).ToListAsync();
                if (optionDependence.Count > 0)
                {
                    var response = optionDependence.Select(opt => new OptionConflictDTO() { Message = $"Cannot delete status \"{opt.Name}\"because it is being used by equipment(s).", Equipment = opt });
                    return Conflict(response);
                }
                
                _equipManageContext.StatusOpt.Remove(option);
                await _equipManageContext.SaveChangesAsync();
                
                return Ok($"Sucessefuly deleted \"{option.Name}\".");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


    }
}