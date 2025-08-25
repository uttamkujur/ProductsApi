using Microsoft.AspNetCore.Mvc;
using Products.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application

{
    public interface IProductService
    {
        Task<ActionResult<ProductDto>> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<bool> AddProductAsync(CreateProductDto dto);
        Task<bool> UpdateProductAsync(UpdateProductDto dto);
        Task<bool> DeleteProductAsync(int id);
        Task<IActionResult> DecrementStockAsync(int id, int quantity);
        Task<IActionResult> IncreaseStockAsync(int id, int quantity);
    }
}



