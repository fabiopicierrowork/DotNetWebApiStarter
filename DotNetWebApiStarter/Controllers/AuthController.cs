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
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;

        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("Register")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterAsync([FromBody][Required] RegisterRequest request, CancellationToken cancellationToken = default)
        {
            if (request.IdRole <= 0)
                ModelState.AddModelError("IdRole", "Role id is mandatory and must be greater than zero.");
            if (string.IsNullOrEmpty(request.Username))
                ModelState.AddModelError("Username", "Username is required.");
            if (string.IsNullOrEmpty(request.Password))
                ModelState.AddModelError("Password", "Password is required.");
            if (string.IsNullOrEmpty(request.ConfirmPassword))
                ModelState.AddModelError("ConfirmPassword", "Confirm password is required.");
            if (!request.Password.Equals(request.ConfirmPassword))
                ModelState.AddModelError("ConfirmPassword", "The confirmation password does not match the password.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                RegisterResponse response = await _authService.RegisterAsync(request, cancellationToken);
                return CreatedAtAction("GetById", "Users", new { id = response.IdUser }, response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to register the user." });
            }
        }

        [HttpPost("Login")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginAsync([FromBody][Required] LoginRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(request.Username))
                ModelState.AddModelError("Username", "Username is required.");
            if (string.IsNullOrEmpty(request.Password))
                ModelState.AddModelError("Password", "Password is required.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                string clientIpAddress = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "UNKNOWN_IP";
                LoginResponse? response = await _authService.LoginAsync(request, clientIpAddress, cancellationToken);

                if (response is null)
                    return Unauthorized();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to login the user." });
            }
        }

        [HttpPost("RefreshToken")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshTokenAsync([FromBody][Required] RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                ModelState.AddModelError("RefreshToken", "Refresh token is required.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                string clientIpAddress = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "UNKNOWN_IP";
                LoginResponse? response = await _authService.RefreshTokenAsync(request.RefreshToken, clientIpAddress, cancellationToken);

                if (response is null)
                    return Unauthorized(new { message = "Invalid or expired refresh token." });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to refresh token." });
            }
        }
    }
}
