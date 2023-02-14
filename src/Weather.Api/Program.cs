using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Weather.Api
{
  public class Program
  {
    public static void Main(string[] args)
    {
      // Настройка кодировки вывода в консоль
      Console.OutputEncoding = System.Text.Encoding.UTF8;

      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
            });
  }
}
