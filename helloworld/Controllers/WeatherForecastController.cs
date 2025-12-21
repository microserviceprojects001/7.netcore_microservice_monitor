using Microsoft.AspNetCore.Mvc;

namespace helloworld.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
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

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("api/testlog")]
    public IActionResult TestLog()
    {
        // 2. 使用 _logger 输出不同级别的日志
        _logger.LogInformation("信息级别：用户访问了 /api/testlog");
        _logger.LogWarning("警告级别：这是一个模拟警告");
        _logger.LogError("错误级别：这是一个模拟错误");

        return Ok(new { message = "日志测试完成，请查看Fluentd和Kibana。" });
    }
}
