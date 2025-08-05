using APIEquipManage.Data;
using APIEquipManage.DTOS;
using APIEquipManage.Extensions;
using APIEquipManage.Helpers;
using APIEquipManage.Models;
using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;

namespace APIEquipManage.Controllers
{
    [ApiController]
    [Route("api/equipment")]
    public class EquipmentController(EquipManageContext equipManageContext) : ControllerBase
    {
        private readonly EquipManageContext _equipManageContext = equipManageContext;

        [HttpGet]
        public async Task<IActionResult> GetEquipment(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortField = "name",
            [FromQuery] string sortOrder = "asc")
        {
            if (page < 1 || pageSize <1 || pageSize > 100){ return BadRequest("Page must be ≥ 1 and pageSize must be between 1 and 100."); }
            try
            {
                
                var equipments = _equipManageContext.Equipment.Include(e => e.StatusOpt).AsNoTracking().OrderByDynamic(sortField, sortOrder);
                var pagedEquipments = await PaginatedList<Equipment>.CreateAsync(equipments, page, pageSize);
                if (pagedEquipments.Items.Count < 1)
                {
                    return NoContent();
                }

                var equipmentsDTO = pagedEquipments.Items.Select(equip => new EquipmentDTO() {
                    Code = equip.Id, 
                    Name = equip.Name, 
                    Model = equip.Model, 
                    Status = equip.StatusOpt?.Name 
                }).ToList();

                var response = new
                {
                    pagedEquipments.HasPreviousPage,
                    pagedEquipments.PageIndex,
                    pagedEquipments.HasNextPage,
                    pagedEquipments.TotalPages,
                    Equipments = equipmentsDTO
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("search")]
        public async Task<IActionResult> GetEquiomentByName(
            [FromQuery] string name,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortField = "Name",
            [FromQuery] string sortOrder = "asc")
        {
            if (page < 1 || pageSize < 1 || pageSize > 100) { return BadRequest("Page must be ≥ 1 and pageSize must be between 1 and 100."); }
            try
            {

                var equipments = _equipManageContext.Equipment.Include(e => e.StatusOpt).AsNoTracking().Where(x => x.Name.Contains(name)).OrderByDynamic(sortField, sortOrder); ;
                var pagedEquipments = await PaginatedList<Equipment>.CreateAsync(equipments, page, pageSize);
                if (pagedEquipments.Items.Count < 1)
                {
                    return NoContent();
                }

                var equipmentsDTO = pagedEquipments.Items.Select(equip => new EquipmentDTO() { Code = equip.Id, Name = equip.Name, Model = equip.Model, Status = equip.StatusOpt?.Name }).ToList();
                var response = new
                {
                    pagedEquipments.HasPreviousPage,
                    pagedEquipments.PageIndex,
                    pagedEquipments.HasNextPage,
                    pagedEquipments.TotalPages,
                    Equipments = equipmentsDTO
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("avaliable")]
        public async Task<IActionResult> GetAvaliableEquipment(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortField = "Name",
            [FromQuery] string sortOrder = "asc")
        {
            if (page < 1 || pageSize < 1 || pageSize > 100) { return BadRequest("Page must be ≥ 1 and pageSize must be between 1 and 100."); }
            try
            {
                var equipments = _equipManageContext.Equipment.AsNoTracking().Where(x => x.IdStatus == 6).OrderByDynamic(sortField, sortOrder); ;

                var pagedEquipments = await PaginatedList<Equipment>.CreateAsync(equipments, page, pageSize);
                if (pagedEquipments.Items.Count < 1)
                {
                    return NoContent();
                }

                var equipmentsDTO = pagedEquipments.Items.Select(equip => new EquipmentDTO() { Code = equip.Id, Name = equip.Name, Model = equip.Model, Status = equip.StatusOpt?.Name }).ToList();
                var response = new
                {
                    pagedEquipments.HasPreviousPage,
                    pagedEquipments.PageIndex,
                    pagedEquipments.HasNextPage,
                    pagedEquipments.TotalPages,
                    Equipments = equipmentsDTO
                };
                return Ok(response);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedEquipment(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortField = "name",
            [FromQuery] string sortOrder = "asc")
        {
            if (page < 1 || pageSize < 1 || pageSize > 100) { return BadRequest("Page must be ≥ 1 and pageSize must be between 1 and 100."); }
            try
            {
                var equipments = _equipManageContext.Equipment.AsNoTracking().Where(x => x.IdStatus == 5).OrderByDynamic(sortField, sortOrder); ;

                var pagedEquipments = await PaginatedList<Equipment>.CreateAsync(equipments, page, pageSize);
                if (pagedEquipments.Items.Count < 1)
                {
                    return NoContent();
                }

                var equipmentsDTO = pagedEquipments.Items.Select(equip => new EquipmentDTO() { Code = equip.Id, Name = equip.Name, Model = equip.Model, Status = equip.StatusOpt?.Name }).ToList();
                var response = new
                {
                    pagedEquipments.HasPreviousPage,
                    pagedEquipments.PageIndex,
                    pagedEquipments.HasNextPage,
                    pagedEquipments.TotalPages,
                    Equipments = equipmentsDTO
                };
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
                var newEquipment = new Equipment()
                {
                    Name = equipment.Name,
                    Model = equipment.Model,
                    Description = equipment.Description,
                    IdStatus = equipment.StatusId,
                    IdCategory = equipment.CategoryId,
                    CreatedAt = DateTime.UtcNow
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
                if (equipment.IdStatus == 6 && updateFields.StatusId != 6)
                {
                    equipment.DeletedAt = null;
                }
                equipment.IdStatus = updateFields.StatusId;
                equipment.IdCategory = updateFields.CategoryId;
                equipment.Name = updateFields.Name;
                equipment.Model = updateFields.Model;
                equipment.Description = updateFields.Description;

                await _equipManageContext.SaveChangesAsync();

                return Ok($"Equipment \"{equipment.Name}\" as ben updated successfully");

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}