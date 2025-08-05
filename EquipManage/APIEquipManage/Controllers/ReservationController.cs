
using APIEquipManage.Data;
using APIEquipManage.Models;
using APIEquipManage.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIEquipManage.Helpers;

namespace APIEquipManage.Controllers
{
    [ApiController]
    [Route("api/reservation")]
    public class ReservationController(EquipManageContext equipManageContext) : ControllerBase
    {
        private readonly EquipManageContext _equipManageContext = equipManageContext;

        [HttpGet]
        public async Task<IActionResult> GetReservations([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100) { return BadRequest("Page must be ≥ 1 and pageSize must be between 1 and 100."); }
            try
            {
                var reservations = _equipManageContext.Reservation.Include(x => x.Equipment).AsNoTracking();
                var pagedReservations = await PaginatedList<Reservation>.CreateAsync(reservations, page, pageSize);

                if (pagedReservations.Items.Count < 1)
                {
                    return NoContent();
                }
                var reservationsDTO = pagedReservations.Items.Select(res => new ReservationDTO()
                {
                    Code = res.Id,
                    Equipment = new EquipmentDTO() { 
                        Code = res.Equipment.Id, 
                        Name = res.Equipment.Name, 
                        Model = res.Equipment.Model },
                    Start = res.StartDate,
                    End = res.EndDate,
                    CanceledAt = res.CanceledAt
                }).ToList();
                var response = new
                {
                    pagedReservations.HasPreviousPage,
                    pagedReservations.PageIndex,
                    pagedReservations.HasNextPage,
                    pagedReservations.TotalPages, 
                    Reservations = reservationsDTO
                };
                
                return Ok(response);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
        [HttpGet("byDateRange")]
        public async Task<IActionResult> GetReservationByDate(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            if (startDate > endDate) { return BadRequest("The date range cannot exceed one month."); }
            if ((endDate - startDate).TotalDays > 31) { return BadRequest("The reservation period cannot exceed one month."); }

            if (page < 1 || pageSize < 1 || pageSize > 100) { return BadRequest("Page must be ≥ 1 and pageSize must be between 1 and 100."); }
            try
            {
                var reservations = _equipManageContext.Reservation.Where(x => x.StartDate >= startDate && x.EndDate <= endDate).Include(x => x.Equipment).AsNoTracking();
                var pagedReservations = await PaginatedList<Reservation>.CreateAsync(reservations, page, pageSize);
                if (pagedReservations.Items.Count < 1)
                {
                    return NoContent();
                }


                var reservationsDTO = pagedReservations.Items.Select(res => new ReservationDTO()
                {
                    Code = res.Id,
                    Equipment = new EquipmentDTO()
                    {
                        Code = res.Equipment.Id,
                        Name = res.Equipment.Name,
                        Model = res.Equipment.Model
                    },
                    Start = res.StartDate,
                    End = res.EndDate,
                    CanceledAt = res.CanceledAt
                }).ToList();
                var response = new
                {
                    pagedReservations.HasPreviousPage,
                    pagedReservations.PageIndex,
                    pagedReservations.HasNextPage,
                    pagedReservations.TotalPages,
                    Reservations = reservationsDTO
                };

                return Ok(response);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpGet("/{id}", Name = "GetReservationById")]
        public async Task<IActionResult> GetReservationById([FromRoute] int Id)
        {
            try
            {
                var reservation = await _equipManageContext.Reservation.FindAsync(Id);
                var response = new ReservationDTO()
                {
                    Code = reservation.Id,
                    Equipment = new EquipmentDTO() { Name = reservation.Equipment.Name, Model = reservation.Equipment.Model },
                    Start = reservation.StartDate,
                    End = reservation.EndDate,
                    CanceledAt = reservation.CreatedAt
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}