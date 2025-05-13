using Microsoft.AspNetCore.Mvc;
using Tutorial9.Model.DTO;
using Tutorial9.Services;

namespace Tutorial9.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(int id)
    {
        var res = await _bookingService.GetBooking(id);
        if (res == null)
            return NotFound("nie ma takiego bookingu");
        
        return Ok(res);
    }

    [HttpPost]
    public async Task<IActionResult> PostBooking([FromBody] PostBookingDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        int res = Convert.ToInt32(await _bookingService.AddBooking(dto));

        switch (res)
        {
            case -1:
                return NotFound("booking already exists");
            case -2:
                return NotFound("guest not found");
            case -3:
                return NotFound("employee not found");
            case -4:
                return NotFound("atraction not found");
            case -5:
                return BadRequest("bad request");
        }
        
        return Ok();
    }
}