using DotNetWebApiStarter.Contracts.Responses;
using DotNetWebApiStarter.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DotNetWebApiStarter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;

        public UsersController(ILogger<UsersController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet("GetById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            GetUserResponse? response = await _userService.GetByIdAsync(id, cancellationToken);
            if (response is null)
                return NotFound();

            return Ok(response);
        }

    }
}
