using Demo.Aspire.Services;
using Demo.Aspire.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Aspire.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class AspNetUsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AspNetUsersController> _logger;
        private readonly IWeatherService _weatherService;

        public AspNetUsersController(UserManager<IdentityUser> userManager,
                                     SignInManager<IdentityUser> signInManager,
                                     ILogger<AspNetUsersController> logger,
                                     IWeatherService weatherService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = await _userManager.GetUserAsync(User);

            return Ok(user);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post(RegisterUser model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var identityUser = new IdentityUser { UserName = model.Email, Email = model.Email };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to create the user {@user}", model);
                return BadRequest(result);
            }

            _logger.LogInformation("User created {@user}", identityUser);
            return Ok(identityUser);
        }

        [HttpGet("underlying-service")]
        [AllowAnonymous]
        public async Task<IActionResult> GetService()
        {
            var response = await _weatherService.GetWeathersAsync();

            return Ok(response);
        }
    }
}
