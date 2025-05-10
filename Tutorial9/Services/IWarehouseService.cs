using Tutorial9.Model.DTO;

namespace Tutorial9.Services;

public interface IWarehouseService
{
    Task<string> AddProduct(ProductDTO dto);
}