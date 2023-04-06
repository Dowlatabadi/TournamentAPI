using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.MQ;
using Tournament.WebAPI.Filters;
using Tournament.WebAPI.Authorization;
using Tournament.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;


namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{

    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        Serilog.ILogger? serilogLogger = new LoggerConfiguration()
        .WriteTo.Console()
        .MinimumLevel.Debug().CreateLogger();

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var serilogelasticLogger = new LoggerConfiguration()
               .Enrich.FromLogContext()
               .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
               {
                   EmitEventFailure = EmitEventFailureHandling.RaiseCallback,
                   BatchPostingLimit = 4000,
                   MinimumLogEventLevel = Serilog.Events.LogEventLevel.Verbose,
                   AutoRegisterTemplate = true,
                   IndexFormat = $"TournamentAPI-{Assembly.GetExecutingAssembly().GetName().Name.ToLower()}-{DateTime.UtcNow:yyyy-MM}"
               })
               .Enrich.WithProperty("Environment", environment)
               .ReadFrom.Configuration(configuration)
               .CreateLogger();
        var microsoftLogger = new SerilogLoggerFactory(serilogelasticLogger)
    .CreateLogger("");
        services.AddSingleton<Microsoft.Extensions.Logging.ILogger>(microsoftLogger);
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.Configure<jwtOptions>(configuration.GetSection("jwt"));
        services.AddSingleton<IJwtUtils, JwtUtils>();
        services.AddSingleton<IMessageConsumerService, MessageConsumerService>();
        services.AddControllers(options =>
                options.Filters.Add<ApiExceptionFilterAttribute>());

        //configuration.GetValue<bool>("UseInMemoryDatabase")
        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);


        services.AddSwaggerGen(option =>
                {
                    option.EnableAnnotations();

                    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Tournament API", Version = "v1" });

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

