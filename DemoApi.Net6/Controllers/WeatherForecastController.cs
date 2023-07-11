using Microsoft.AspNetCore.Mvc;
using Hei.Infrastructure;

namespace DemoApi.Net6.Controllers
{
    public class WeatherForecastController : HeiApiController
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(HeiApiResult<IEnumerable<WeatherForecast>>), 200)]
        public IActionResult Get()
        {
            return Success("success", Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)] + AppSettings.Get("AllowedHosts", false)
            })
            .ToArray());
        }
    }
}