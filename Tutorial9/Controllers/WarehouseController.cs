using Microsoft.AspNetCore.Mvc;
using Tutorial9.Model.DTO;
using Tutorial9.Services;

namespace Tutorial9.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private IWarehouseService _warehouseService;
    
    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] ProductDTO dto)
    {
        if (dto.Amount <= 0)
            return BadRequest("Amount must be greater than zero");

        string result = await _warehouseService.AddProduct(dto); //todo
        return Ok();
    }
}