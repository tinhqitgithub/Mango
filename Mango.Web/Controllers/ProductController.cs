using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
  public class ProductController : Controller
  {
    private readonly IProductService _productService;
    private readonly ILogger<CartController> _logger;

    public ProductController(IProductService productService, ILogger<CartController> logger)
    {
      _productService = productService;
      _logger = logger;
    }

    public async Task<IActionResult> ProductIndex()
    {
      _logger.LogInformation("Product123");
      List<ProductDto> products = new();
      var responseDto = await _productService.GetAllProductsAsync();
      if (responseDto != null && responseDto.IsSuccess)
      {
        products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(responseDto.Result));
      }
      else
      {
        TempData["error"] = responseDto?.Message;
      }
      return View(products);
    }

    public async Task<IActionResult> ProductCreate()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> ProductCreate(ProductDto model)
    {
      if (ModelState.IsValid)
      {
        ResponseDto? response = await _productService.CreateProductAsync(model);
        if (response != null && response.IsSuccess)
        {
          TempData["success"] = "Product Created Successful.";
          return RedirectToAction("ProductIndex");
        }
        else
        {
          TempData["error"] = response?.Message;
        }

      }
      return View(model);
    }

    public async Task<IActionResult> ProductUpdate(int productId)
    {
      var responseDto = await _productService.GetProductByIdAsync(productId);
      if (responseDto != null && responseDto.IsSuccess)
      {
        ProductDto dto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(responseDto.Result));
        return View(dto);
      }
      else
      {
        TempData["error"] = responseDto?.Message;
        return NotFound();
      }
    }

    [HttpPost]
    public async Task<IActionResult> ProductUpdate(ProductDto model)
    {
      if (ModelState.IsValid)
      {
        ResponseDto? response = await _productService.UpdateProductAsync(model);
        if (response != null && response.IsSuccess)
        {
          TempData["success"] = "Product updated Successful.";
          return RedirectToAction("ProductIndex");
        }
        else
        {
          TempData["error"] = response?.Message;
        }

      }
      return View(model);
    }

    public async Task<IActionResult> ProductDelete(int productId)
    {
      ResponseDto? response = await _productService.GetProductByIdAsync(productId);
      if (response != null && response.IsSuccess)
      {
        ProductDto dto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
        return View(dto);
      }
      else
      {
        TempData["error"] = response?.Message;
        return NotFound();
      }
    }

    [HttpPost]
    public async Task<IActionResult> ProductDelete(ProductDto model)
    {
      ResponseDto? response = await _productService.DeleteProductAsync(model.ProductId);
      if (response != null && response.IsSuccess)
      {
        TempData["success"] = "Product deleted Successful.";
        return RedirectToAction("ProductIndex");
      }
      else
      {
        TempData["error"] = response?.Message;
        return View(model);
      }
    }
  }
}
