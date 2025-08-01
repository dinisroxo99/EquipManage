using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIEquipManage.Data;
using APIEquipManage.Models;
using System;
using Microsoft.IdentityModel.Tokens;
using APIEquipManage.Extensions;
using APIEquipManage.DTOS;

[ApiController]
[Route("api/equioment")]
public class EquipmentController : ControllerBase
{
    private readonly EquipManageContext _equipManageContext;

    public EquipmentController(EquipManageContext equipManageContext)
    {
        _equipManageContext = equipManageContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetEquipment()
    {
        try
        {
            var equipments = await _equipManageContext.Equipment.IncludeAll().AsNoTracking().ToListAsync();
            if (equipments.Count == 0)
            {
                return NoContent();
            }
            return Ok(equipments);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
        
   
        
    }
    [HttpGet("{name}")]
    public async Task<IActionResult> GetEquiomentByName(string name)
    {
        try
        {
            var equipment = await _equipManageContext.Equipment.IncludeAll().AsNoTracking().FirstOrDefaultAsync(x => x.Name == name);
            if (equipment == null)
            {
                return NotFound();
            }
            return Ok(equipment);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpGet("/avaliable")]
    public async Task<IActionResult> GetEquipmentAvaliable()
    {
        try
        {
            var equipments = await _equipManageContext.Equipment.IncludeAll()
                                                                .AsNoTracking()
                                                                .Where(x => x.StatusOpt.Name == "avaliable")
                                                                .ToListAsync();

            if (equipments.Count == 0) { return NoContent(); }
            return Ok(equipments);
        }
        catch (Exception e)
        {

            return BadRequest(e.Message);
        }
    }
    [HttpPost]
    public async Task<IActionResult> NewEquipment([FromBody] Equipment equipment)
    {
        try
        {
            if (string.IsNullOrEmpty(equipment.Name))
            {
                equipment.Name = Guid.NewGuid().ToString();
            }
            equipment.CreatedAt = DateTime.UtcNow;
            _equipManageContext.Equipment.Add(equipment);
            await _equipManageContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEquipment), new { name = equipment.Name }, equipment);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    /********************************
     *                              *
     *      É preciso de rever isto *
     *                              *
     * ******************************/
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEquipment(int id)
    {
        var equipment = await _equipManageContext.Equipment.FindAsync(id);
        if (equipment == null)
        {
            return Forbid();
        }
        var canceledReservations = new List<Reservation>(); 
        var reservation = await _equipManageContext.Reservation.Where(r => r.IdEquipment == id).ToListAsync();
        if (reservation.Count > 0)
        {
            foreach (var r in reservation)
            {
                if (r.EndDate > DateTime.UtcNow)
                {
                    r.CanceledAt = DateTime.UtcNow;
                    canceledReservations.Add(r);
                }
            }
        }
        var deletedStatus = await _equipManageContext.StatusOpt.FirstOrDefaultAsync(x => x.Name == "Deleted");
        if (deletedStatus == null)
        {
            return BadRequest(new { message = "Can't find the option to Delete" });
        }
        equipment.IdStatus = deletedStatus.Id;
        await _equipManageContext.SaveChangesAsync();
        return Ok(canceledReservations);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEquipment(int id, [FromBody] UpdateEquipmentDTO updateFields)
    {
        try
        {
            var equipment = await _equipManageContext.Equipment.FindAsync(id);
            if (equipment == null)
                return NotFound();
            
            equipment.IdStatus = updateFields.StatusId;
            equipment.IdCategory = updateFields.CategoryId;
            equipment.Name = updateFields.Name;
            equipment.Model = updateFields.Model;
            equipment.Description = updateFields.Description;

            await _equipManageContext.SaveChangesAsync();

            return Ok(equipment);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
          
}

