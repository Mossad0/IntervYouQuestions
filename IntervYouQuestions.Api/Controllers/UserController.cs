using IntervYouQuestions.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntervYouQuestions.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger; // ✅ إضافة اللوجينج

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid register request received.");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation($"Attempting to register user with email: {request.Email}");

                var result = await _userService.RegisterUserAsync(request);
                if (result == null)
                {
                    _logger.LogWarning($"Registration failed: Email '{request.Email}' is already taken.");
                    return BadRequest("User with this email already exists.");
                }

                _logger.LogInformation($"User registered successfully: {request.Email}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during registration for email {request.Email}: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login request received.");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation($"Attempting to log in user with email: {request.Email}");

                var result = await _userService.LoginAsync(request);
                if (result == null)
                {
                    _logger.LogWarning($"Login failed for email: {request.Email}. Invalid credentials.");
                    return Unauthorized("Invalid email or password.");
                }

                _logger.LogInformation($"User logged in successfully: {request.Email}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during login for email {request.Email}: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}

