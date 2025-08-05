
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
        public async Task<IActionResult> GetReservations([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string sort = "desc")
        {
            if (page < 1 || pageSize < 1 || pageSize > 100) { return BadRequest("Page must be ≥ 1 and pageSize must be between 1 and 100."); }
            try
            {
                var reservations = _equipManageContext.Reservation.Include(x => x.Equipment).AsNoTracking();
                reservations = sort.Equals("asc", StringComparison.CurrentCultureIgnoreCase) ? reservations.OrderBy(r => r.StartDate) : reservations.OrderByDescending(r => r.StartDate);
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
            [FromQuery] int pageSize = 10,
            [FromQuery] string sort = "desc")
        {
            if (startDate > endDate) { return BadRequest("The date range cannot exceed one month."); }
            if ((endDate - startDate).TotalDays > 31) { return BadRequest("The reservation period cannot exceed one month."); }

            if (page < 1 || pageSize < 1 || pageSize > 100) { return BadRequest("Page must be ≥ 1 and pageSize must be between 1 and 100."); }
            try
            {
                var reservations = _equipManageContext.Reservation.Where(x => x.StartDate >= startDate && x.EndDate <= endDate).Include(x => x.Equipment).AsNoTracking();
                reservations = sort.Equals("asc", StringComparison.CurrentCultureIgnoreCase) ? reservations.OrderBy(r => r.StartDate) : reservations.OrderByDescending(r => r.StartDate);
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

        [HttpGet("{id}", Name = "GetReservationById")]
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

        [HttpPut("Id")]
        public async Task<IActionResult> UpdateReservation([FromRoute] int Id, [FromBody] UpdateReservationDTO reservationDTO)
        {
            if (reservationDTO == null) { return BadRequest("No content found to update"); }
            if (reservationDTO.Start > reservationDTO.End || reservationDTO.Start < DateTime.UtcNow) { return BadRequest("message"); }
            
            try
            {
                var reservation = await _equipManageContext.Reservation.FindAsync(Id);
                if (reservation == null)
                {
                    return NotFound();
                }
                var reservationsDependences = await _equipManageContext.Reservation.Where(x => x.IdEquipment == reservationDTO.IdEquipment).ToListAsync();
                bool hasOverLap = reservationsDependences.Any(existing =>
                    reservationDTO.Start < existing.EndDate &&
                    reservationDTO.End > existing.StartDate);

                if (hasOverLap)
                {
                    return BadRequest("There is already a reservation in that time slot.");
                }
                reservation.StartDate = reservationDTO.Start;
                reservation.EndDate = reservationDTO.End;

                await _equipManageContext.SaveChangesAsync();

                return Ok($"Update sucessefully reservation strats at: {reservation.StartDate.Date}; and ends at: {reservation.EndDate.Date}");
                


            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
        [HttpPost()]
        public async Task<IActionResult> NewReservation([FromBody] NewReservationDTO newReservationDTO)
        {
            if (newReservationDTO.Start < DateTime.UtcNow) { return BadRequest("The reservation start date and time cannot be in the past."); }
            if (newReservationDTO.End < newReservationDTO.Start) { return BadRequest("The reservation end date must be after the start date."); }
            try
            {
                var equipmentDependence = await _equipManageContext.Reservation.Where(x => x.IdEquipment == newReservationDTO.IdEquipment).ToListAsync();

                bool hasOverLap = equipmentDependence.Any(existing =>
                    newReservationDTO.Start < existing.EndDate &&
                    newReservationDTO.End > existing.StartDate);

                if (hasOverLap)
                {
                    return BadRequest("There is already a reservation in that time slot.");
                }


                var newReservation = new Reservation()
                {
                    IdEquipment = newReservationDTO.IdEquipment,
                    CreatedAt = DateTime.UtcNow,
                    StartDate = newReservationDTO.Start,
                    EndDate = newReservationDTO.End
                };
                _equipManageContext.Reservation.Add(newReservation);
                await _equipManageContext.SaveChangesAsync();

                //return Ok("Created Successfully");
                return CreatedAtRoute("GetReservationById", new { id = newReservation.Id }, newReservation);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}