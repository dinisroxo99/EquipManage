﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIEquipManage.Data;
using APIEquipManage.Models;
using System;
using Microsoft.IdentityModel.Tokens;
using APIEquipManage.Extensions;
using APIEquipManage.DTOS;

[ApiController]
[Route("api/equipment")]
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
            var equipments = await _equipManageContext.Equipment.Include(e => e.StatusOpt).ToListAsync();
            if (equipments.Count == 0)
            {
                return NoContent();
            }
            List<GetEquipmentDTO> response = new List<GetEquipmentDTO>();
            foreach (var equip in equipments)
            {
                response.Add(new GetEquipmentDTO() { Code = equip.Id, Name = equip.Name, Model = equip.Model, Status=equip.StatusOpt.Name });
            }
            return Ok(response);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
        
   
        
    }
    [HttpGet("search")]
    public async Task<IActionResult> GetEquiomentByName([FromQuery] string name)
    {
        try
        {
            var equipment = await _equipManageContext.Equipment.Include(e => e.StatusOpt).AsNoTracking().Where(x => x.Name.Contains(name)).ToListAsync();
            if (equipment == null)
            {
                return NoContent();
            }
            List<GetEquipmentDTO> response = new List<GetEquipmentDTO>();
            foreach (var equip in equipment) 
            { 
                response.Add(new GetEquipmentDTO()
                {
                    Code = equip.Id,
                    Name = equip.Name,
                    Model = equip.Model,
                    Status = equip.StatusOpt.Name
                }); 
            }
            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpGet("avaliable")]
    public async Task<IActionResult> GetAvaliableEquipment()
    {
        try
        {
            var equipments = await _equipManageContext.Equipment.AsNoTracking()
                                                                .Where(x => x.IdStatus == 6)
                                                                .ToListAsync();

            if (equipments.Count < 1) { return NoContent(); }
            List<GetEquipmentDTO> response = new List<GetEquipmentDTO>();
            foreach (var equip in equipments)
            {
                var equipDTO = new GetEquipmentDTO() { Code = equip.Id, Name = equip.Name, Model = equip.Model };
                response.Add(equipDTO);
            }
            return Ok(response);
        }
        catch (Exception e)
        {

            return BadRequest(e.Message);
        }
    }
    [HttpGet("deleted")]
    public async Task<IActionResult> GetDeletedEquipment()
    {
        try
        {
            var equipments = await _equipManageContext.Equipment.AsNoTracking()
                                                                .Where(x => x.IdStatus == 5)
                                                                .ToListAsync();

            if (equipments.Count < 1) { return NoContent(); }
            List<GetEquipmentDTO> response = new List<GetEquipmentDTO>();
            foreach (var equip in equipments)
            {
                var equipDTO = new GetEquipmentDTO() { Code = equip.Id, Name = equip.Name, Model = equip.Model };
                response.Add(equipDTO);
            }
            return Ok(response);
        }
        catch (Exception e)
        {

            return BadRequest(e.Message);
        }
    }
    [HttpPost]
    public async Task<IActionResult> NewEquipment([FromBody] NewEquipmentDTO equipment)
    {
        try
        {
            var newEquipment = new Equipment() {
                Name=equipment.Name, 
                Model=equipment.Model, 
                Description=equipment.Description,
                IdStatus=equipment.StatusId,
                IdCategory=equipment.CategoryId,
                CreatedAt=DateTime.UtcNow
            };
            _equipManageContext.Equipment.Add(newEquipment);
            await _equipManageContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEquipment), new { name = newEquipment.Name }, newEquipment);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
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
        var response = new DeletedEquipmentDTO()
        {
            Code = equipment.Id,
            Name = equipment.Name,
            Model = equipment.Model,
            Status = equipment.StatusOpt.Name,
            CanceledReservation = canceledReservations
        };
        await _equipManageContext.SaveChangesAsync();
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEquipment(int id, [FromBody] UpdateEquipmentDTO updateFields)
    {
        try
        {
            var equipment = await _equipManageContext.Equipment.FindAsync(id);
            if (equipment == null)
                return NotFound();
            
            equipment.IdStatus = updateFields.StatusId.Value;
            equipment.IdCategory = updateFields.CategoryId.Value;
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

