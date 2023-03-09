using Tournament.Application.Common.Interfaces;
using Tournament.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Microsoft.Data.SqlClient;

namespace Tournament.Application.IntegrationTests;
using static Testing;

internal class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureAppConfiguration(configurationBuilder =>
				{
				var integrationConfig = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables()
				.Build();

				configurationBuilder.AddConfiguration(integrationConfig);
				});

		builder.ConfigureServices((builder, services) =>
				{

				services
				.Remove<ICurrentUserService>()
				.AddTransient(provider => Mock.Of<ICurrentUserService>(s =>
							s.UserName == GetCurrentUserId()));


				var connectionstring= new  SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"));
				services
				.Remove<DbContextOptions<ApplicationDbContext>>()
				.AddDbContext<ApplicationDbContext>((sp, options) =>
						options.UseSqlServer(connectionstring,
							builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
				});
	}
}



