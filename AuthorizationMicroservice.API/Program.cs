using AuthorizationMicroservice.API.Infrastructure;
using AuthorizationMicroservice.Application.CryptographyService;
using AuthorizationMicroservice.Domain.Models;
using AuthorizationMicroservice.Persistance.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace AuthorizationMicroservice.API
{
    public class Program
    {
        public static async System.Threading.Tasks.Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var repo = services.GetRequiredService<IUserRepository>();
                var configuration = services.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
                SuperAdminHandler superAdminHandler = new SuperAdminHandler(repo, configuration);
                try
                {
                    await superAdminHandler.LoadSuperAdminAsync();
                }
                catch (Exception)
                {
                    await superAdminHandler.CreateSuperAdminAsync();
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
