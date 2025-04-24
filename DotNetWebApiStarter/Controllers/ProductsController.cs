using DotNetWebApiStarter.DTOs.Requests;
using DotNetWebApiStarter.DTOs.Responses;
using DotNetWebApiStarter.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DotNetWebApiStarter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductService _productService;

        public ProductsController(ILogger<ProductsController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery][Required] int pageNumber, [FromQuery][Required] int pageSize, CancellationToken cancellationToken = default)
        {
            IEnumerable<InsertProductResponseDTO> response = await _productService.GetAllAsync(pageNumber, pageSize, cancellationToken);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            InsertProductResponseDTO? response = await _productService.GetByIdAsync(id, cancellationToken);
            if (response is null)
                return NotFound();

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> InsertAsync([FromBody][Required] InsertProductRequestDTO request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            InsertProductResponseDTO response = await _productService.InsertAsync(request, cancellationToken);
            return Ok(response);
        }
    }
}
