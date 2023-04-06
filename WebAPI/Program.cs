using Tournament.Infrastructure.Persistence;
using Tournament.WebAPI.Middlewares;
using Tournament.Application.Common.MQ;
using Swashbuckle.AspNetCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebAPIServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Detailed Logs using 3rd pkg
//Serilog.ILogger? logger = new LoggerConfiguration()
//		.WriteTo.Console()
//		.MinimumLevel.Debug().CreateLogger();
//builder.Host.UseSerilog(logger);
var app = builder.Build();
//app.UseSerilogRequestLogging();

var MQconsumer=app.Services.GetService<IMessageConsumerService>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    // Initialise and seed database
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync();
		await initialiser.TrySeedUsersAsync();
        await initialiser.SeedAsync();
    }
}
else
{
	// seed users only
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
		await initialiser.TrySeedUsersAsync();
    }
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthorization();
app.UseMiddleware<JwtMiddleware>();
app.MapControllers();

app.Run();
public partial class Program { }
