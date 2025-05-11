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

        int result = await _warehouseService.AddProduct(dto);

        switch (result)
        {
            case -1:
                return NotFound("Product not found");
            case -2:
                return NotFound("Warehouse not found");
            case -3:
                return BadRequest("No order for this product");
            case -4:
                return BadRequest("Order already fulfilled");
            default:
                return Ok(result);
        }
    }
}