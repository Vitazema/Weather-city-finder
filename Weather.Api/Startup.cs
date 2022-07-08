using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Weather.Api.Services;
using Weather.Data;

namespace Weather.Api
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers();

      // Настройка соединения с локальной MySQL
      var mySqlConnectionString = Configuration.GetConnectionString("WeatherConnection");

      services.AddDbContext<WeatherDbContext>(opt =>opt
        .UseMySql(mySqlConnectionString, ServerVersion.AutoDetect(mySqlConnectionString), options => options.MaxBatchSize(100))
        .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Name }, LogLevel.Information)
        .LogTo(log => Debug.WriteLine(log), LogLevel.Information)
        .EnableSensitiveDataLogging()
        );
      services.AddHostedService<WeatherService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
