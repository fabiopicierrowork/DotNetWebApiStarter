using DotNetWebApiStarter.Contracts.Requests;
using DotNetWebApiStarter.Contracts.Responses;
using DotNetWebApiStarter.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync([FromQuery][Required] int pageNumber, [FromQuery][Required] int pageSize, CancellationToken cancellationToken = default)
        {
            IEnumerable<GetProductResponse> response = await _productService.GetAllAsync(pageNumber, pageSize, cancellationToken);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            GetProductResponse? response = await _productService.GetByIdAsync(id, cancellationToken);
            if (response is null)
                return NotFound();

            return Ok(response);
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InsertAsync([FromBody][Required] CreateProductRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(request.Name))
                ModelState.AddModelError("Name", "Product name is required.");
            if (request.Price <= 0)
                ModelState.AddModelError("Price", "Product price is mandatory and must be greater than zero.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CreateProductResponse response = await _productService.InsertAsync(request, cancellationToken);
            return Created(Url.Action(nameof(GetByIdAsync), new { id = response.Id }), response);
        }

        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody][Required] UpdateProductRequest request, CancellationToken cancellationToken = default)
        {
            if (id != request.Id)
                return BadRequest("The ID in the route does not match the ID in the request body.");
            if (string.IsNullOrEmpty(request.Name))
                ModelState.AddModelError("Name", "Product name is required.");
            if (request.Price <= 0)
                ModelState.AddModelError("Price", "Product price is mandatory and must be greater than zero.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            GetProductResponse? existingProduct = await _productService.GetByIdAsync(id, cancellationToken);
            if (existingProduct is null)
                return NotFound();

            bool updated = await _productService.UpdateAsync(request, cancellationToken);

            if (updated)
                return NoContent();
            else
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update the product.");
        }
    }
}
