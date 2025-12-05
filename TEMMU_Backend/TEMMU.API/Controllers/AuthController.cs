using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TEMMU.API.Models;
using TEMMU.API.Services;


namespace TEMMU.API.Controllers
{

    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        // NOTE: This endpoint is public
        public async Task<IActionResult> Register(RegistrationDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDTO { isSuccess = false, ErrorMessage = "Invalid registration data." });
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Optionally add roles here if needed
                return Ok(new AuthResponseDTO { isSuccess = true, ErrorMessage = "Registration successful." });
            }

            // Handle Identity errors
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new AuthResponseDTO { isSuccess = false, ErrorMessage = $"Registration failed: {errors}" });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDTO { isSuccess = false, ErrorMessage = "Invalid login data." });
            }

            // 1. Check if user exists by email
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return Unauthorized(new AuthResponseDTO { isSuccess = false, ErrorMessage = "Invalid credentials." });
            }

            // 2. Validate password
            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
            {
                return Unauthorized(new AuthResponseDTO { isSuccess = false, ErrorMessage = "Invalid credentials." });
            }

            // 3. If valid, generate JWT
            var token = _tokenService.CreateToken(user);

            // Return 200 OK with the token
            return Ok(new AuthResponseDTO
            {
                isSuccess = true,
                token = token
            });
        }
    }
}
