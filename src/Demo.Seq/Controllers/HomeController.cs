using Demo.Seq.Services;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Seq.Controllers
{
    [ApiController]
    [Route("api/v1/home")]
    public class HomeController : ControllerBase
    {
        private readonly IWeatherService _wheatherService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IWeatherService wheatherService, ILogger<HomeController> logger)
        {
            _wheatherService = wheatherService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetWheathers()
        {
            _logger.LogInformation("ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ");

            var wheathers = await _wheatherService.GetWeathersAsync();

            return Ok(wheathers);
        }
    }
}
