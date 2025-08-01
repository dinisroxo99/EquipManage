
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
            return Ok(reservations);
        }
        catch (Exception e)
        {

            return BadRequest(e.Message);
        }
    }

    private IActionResult BadRequest(string message)
    {
        throw new NotImplementedException();
    }
}

