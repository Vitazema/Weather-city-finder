using BoDi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weather.Api;
using Weather.Api.Configuration;

namespace Weather.Specs.Hooks;

[Binding]
public class Hooks
{
    private const string AppSettingsFile = "appsettings.json";
    private const string baseAddress = "http://localhost/";
    private static WebApplicationFactory<Startup> _factory;
    private readonly IObjectContainer _objectContainer;
    private readonly ScenarioContext _scenarioContext;

    public Hooks(IObjectContainer objectContainer, ScenarioContext scenarioContext)
    {
        _objectContainer = objectContainer;
        _scenarioContext = scenarioContext;
    }
    
    /// <summary>
    ///     Setup the test infrastructure.
    /// </summary>
    [BeforeTestRun]
    public static void SetupTestRun()
    {
        _factory = new WebApplicationFactory<Startup>()
            .WithWebHostBuilder(builder =>
            {
                IConfigurationSection configSection = null;
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), AppSettingsFile));
                    configSection = context.Configuration.GetSection(nameof(WeatherDatabaseSettings));
                });
                builder.ConfigureTestServices(
                    services => services.Configure<WeatherDatabaseSettings>(configSection));
            });
    }

    [BeforeScenario]
    public void InjectMocks()
    {
        _objectContainer.RegisterInstanceAs(_factory);
        var httpClient = _factory.CreateDefaultClient(new Uri(baseAddress));
        _objectContainer.RegisterInstanceAs(httpClient);
    }
    // private static IConfiguration LoadConfiguration()
    // {
    //     return new ConfigurationBuilder()
    //         .AddJsonFile(AppSettingsFile)
    //         .Build();
    // }

    // private static void ConfigureTestServices(IServiceCollection services)
    // {
    //     services.AddScoped()
    // }

    // private async Task EnsureDatabase(WebApplicationFactory<Program> factory)
    // {
    //     if (factory.Services.GetService(typeof(IWeatherRepository)) 
    //         is not IWeatherRepository weatherRepository) return;
    //     await weatherRepository.RemoveWeathers();
    //     var weathers = await new InMemoryWeatherRepository().GetWeathers();
    //     await weatherRepository.AddWeathers(weathers);
    // }
}