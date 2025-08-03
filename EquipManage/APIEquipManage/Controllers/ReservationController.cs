
using APIEquipManage.Data;
using APIEquipManage.Models;
using APIEquipManage.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/reservation")]
public class ReservationController : ControllerBase
{
    private readonly EquipManageContext _equipManageContext;

    public ReservationController(EquipManageContext equipManageContext)
    {
        _equipManageContext = equipManageContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetReservations()
    {
        try
        {
            var reservations = await _equipManageContext.Reservation.AsNoTracking().ToListAsync();
            if (reservations.Count < 1)
            {
                return NoContent();
            }
            var response = new List<GetReservationDTO>();
            foreach (var item in reservations)
            {
                var equipmentDTO = new GetEquipmentDTO() { Code = item.Equipment.Id, Name = item.Equipment.Name, Model = item.Equipment.Model };

                response.Add(new GetReservationDTO() { Code = item.Id, Equipment = equipmentDTO, Start = item.StartDate, End = item.EndDate });
            }
            return Ok(response);
        }
        catch (Exception e)
        {

            return BadRequest(e.Message);
        }
    }

}

