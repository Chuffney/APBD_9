using Microsoft.AspNetCore.Mvc;
using Tutorial9.Model.DTO;

namespace Tutorial9.Services;

public interface IWarehouseService
{
    Task<int> AddProduct(ProductDTO dto);
}