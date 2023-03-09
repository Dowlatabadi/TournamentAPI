using Tournament.Application.Common.Interfaces;
using Tournament.WebAPI.Filters;
using Tournament.WebAPI.Authorization;
using Tournament.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog.Extensions.Logging;
using Serilog;


namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{

    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        Serilog.ILogger? serilogLogger = new LoggerConfiguration()
        .WriteTo.Console()
        .MinimumLevel.Debug().CreateLogger();
        var microsoftLogger = new SerilogLoggerFactory(serilogLogger)
    .CreateLogger("");
        services.AddSingleton<Microsoft.Extensions.Logging.ILogger>(microsoftLogger);
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        jwtOptions? jwtoptions = configuration.GetSection("jwt").Get<jwtOptions>();
        services.AddSingleton<jwtOptions>(jwtoptions);
        services.AddSingleton<IJwtUtils, JwtUtils>();

        services.AddControllers(options =>
                options.Filters.Add<ApiExceptionFilterAttribute>());

        //configuration.GetValue<bool>("UseInMemoryDatabase")
        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);


        services.AddSwaggerGen(option =>
                {
					option.EnableAnnotations();

                    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });

                    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter a valid token",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "Bearer"
                    });

                    option.AddSecurityRequirement(new OpenApiSecurityRequirement
                        {
                        {
                        new OpenApiSecurityScheme
                        {
                        Reference = new OpenApiReference
                        {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                        }
                        },
                        new string[]{}
                        }
                        });
                });


        return services;
    }
}

