using Tournament.Application.Common.Interfaces;
using Tournament.Infrastructure.Identity;
using Tournament.Infrastructure.Persistence;
using Tournament.Infrastructure.Persistence.Interceptors;
using Tournament.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<AuditableEntitySaveChangesInterceptor>();

		if (configuration.GetValue<bool>("UseInMemoryDatabase"))
		{
			services.AddDbContext<ApplicationDbContext>(options =>
					options.UseInMemoryDatabase("TDb"));
		}
		else
		{
			services.AddDbContext<ApplicationDbContext>(options =>
					//   options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
					options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
						builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
					}
					services.AddIdentity<ApplicationUser, IdentityRole>()
					.AddEntityFrameworkStores<ApplicationDbContext>();

					services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

					services.AddScoped<ApplicationDbContextInitialiser>();

					services.AddSingleton<IDrawService>(new RandomDrawService());
					services.AddTransient<IDateTime, DateTimeService>();
					services.AddTransient<IIdentityService,IdentityService>();

					services.AddAuthorization(options =>
						options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator")));

					return services;
					}
					}

