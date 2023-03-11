using EventStore.Client;
using Eventuous;
using Eventuous.EventStore;
using Microsoft.AspNetCore.Mvc;

namespace EventuousCustomerDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
        private readonly IServiceCollection _services;
        private readonly IAggregateStore _aggregateStore;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(
            IServiceCollection services,
            IAggregateStore aggregateStore,
            ILogger<WeatherForecastController> logger)
        {
            _services = services;
            _aggregateStore = aggregateStore;
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

        [HttpGet("services")]
        public IEnumerable<string> GetServices()
        {
            var x = new EventStoreClient();
            AggregateFactoryRegistry aggregateFactoryRegistry = new AggregateFactoryRegistry();
            EsdbEventStore esdbEventStore;
            var result = x.ReadStreamAsync(Direction.Forwards, "", StreamPosition.Start);
            //result.Select(e => e.Event.)

            return _services.Select(s => s.ServiceType.Name);
        }
    }
}