using Tournament.Infrastructure.Identity;
using Tournament.Infrastructure.Persistence;
using Tournament.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Respawn;
using Respawn.Graph;
using Microsoft.Data.SqlClient;

namespace Tournament.Application.IntegrationTests;

[SetUpFixture]
public partial class Testing
{
	private static WebApplicationFactory<Program> _factory = null!;
	private static IConfiguration _configuration = null!;
	public static IServiceScopeFactory _scopeFactory = null!;
	private static SqlConnection connection = null!;
	private static Respawner _checkpoint = null!;
	private static string? _currentUserId;

	[OneTimeSetUp]
	public void RunBeforeAnyTests()
	{
		_factory = new CustomWebApplicationFactory();
		_scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
		_configuration = _factory.Services.GetRequiredService<IConfiguration>();
		connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
		connection.Open();
		_checkpoint = Respawner.CreateAsync(connection, new RespawnerOptions
				{
				TablesToIgnore = new Table[] { "__EFMigrationsHistory" },
				DbAdapter = DbAdapter.SqlServer,
				WithReseed=true//truncates Ids
				}).Result;
	}
	public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
	{
		using var scope = _scopeFactory.CreateScope();

		var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

		return await mediator.Send(request);
	}

	public static string? GetCurrentUserId()
	{
		return _currentUserId;
	}

	public static void SeedData()
	{
		using var scope = _scopeFactory.CreateScope();

		var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
		context.Channels.Add(
				new Channel{
				Title="Channel title 1",
				Contests={
				new Contest { Title = "Contest1",WinnersCapacity=3,WeightedDraw=true,ParticipationCapacity=100,Start=DateTime.Now.AddDays(-10),Finish=DateTime.Now.AddDays(10),

                Questions =
				{
				new Question { Title = "Who did make Contest1?" , 
				Options = {new Option {Title="1-Option Number 1 for Q1."},new Option {Title="2-Option Number 2 for Q1."}}},
				new Question{ Title = "When the Contest1 was made?", 
				Options = {new Option {Title="1-Option Number 1 for Q2."},new Option {Title="2-Option Number 2 for Q2."}}},
				new Question{ Title = "Where the Contest1 was made?",
				Options = {new Option {Title="1-Option Number 1 for Q3."},new Option {Title="2-Option Number 2 for Q3."}}},
				new Question{ Title = "Whose Contest is this?" ,
				Options = {new Option {Title="1-Option Number 1 for Q4."}}},
				},
				Participations=
				{
				new Participation{ AccountId="dsf43#",  Spent=2.35d}
				,
				new Participation{ AccountId="000000",  Spent=.002d  } 
				}	
				},
				//past
                new Contest { Title = "past Contest",WinnersCapacity=3,WeightedDraw=true,ParticipationCapacity=100,Start=DateTime.Now.AddDays(-10),Finish=DateTime.Now.AddDays(-8) }
				,
				//future
                new Contest { Title = "future Contest",WinnersCapacity=3,WeightedDraw=true,ParticipationCapacity=100,Start=DateTime.Now.AddDays(+10),Finish=DateTime.Now.AddDays(+12) }

                    }
                });
		context.SaveChanges();
		context.Answers.AddRange(new List<Answer>{new Answer { OptionId=1, ParticipationId=1 },new Answer { OptionId=3, ParticipationId=1 },new Answer { OptionId=5, ParticipationId=1 },new Answer { OptionId=7, ParticipationId=1 }});
		context.Answers.AddRange(new List<Answer>{new Answer { OptionId=2, ParticipationId=2 },new Answer { OptionId=3, ParticipationId=2 },new Answer { OptionId=5, ParticipationId=2 },new Answer { OptionId=7, ParticipationId=2 }});
		context.SaveChanges();
	}

	public static async Task<string> RunAsDefaultUserAsync()
	{
		return await RunAsUserAsync("test@local", "Testing1234!", Array.Empty<string>());
	}

	public static async Task<string> RunAsAdministratorAsync()
	{
		return await RunAsUserAsync("administrator@local", "Administrator1234!", new[] { "Administrator" });
	}

	public static async Task<string> RunAsUserAsync(string userName, string password, string[] roles)
	{
		using var scope = _scopeFactory.CreateScope();

		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
		var usrs = userManager.Users.ToList();
		var user = new ApplicationUser { UserName = userName, Email = userName };

		var result = await userManager.CreateAsync(user, password);

		if (roles.Any())
		{
			var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			foreach (var role in roles)
			{
				await roleManager.CreateAsync(new IdentityRole(role));
			}

			await userManager.AddToRolesAsync(user, roles);
		}

		if (result.Succeeded)
		{
			_currentUserId = user.Id;

			return _currentUserId;
		}

		var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);

		throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
	}

	public static async Task ResetState()
	{
		await _checkpoint.ResetAsync(connection);
		_currentUserId = null;
	}

	public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
		where TEntity : class
		{
			using var scope = _scopeFactory.CreateScope();

			var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

			return await context.FindAsync<TEntity>(keyValues);
		}

	public  static ApplicationDbContext GetContext()
	{
		var scope = _scopeFactory.CreateScope();

		return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	}

	public static async Task AddAsync<TEntity>(TEntity entity)
		where TEntity : class
		{
			using var scope = _scopeFactory.CreateScope();

			var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

			context.Add(entity);

			await context.SaveChangesAsync();
		}

	public static async Task<int> CountAsync<TEntity>() where TEntity : class
	{
		using var scope = _scopeFactory.CreateScope();

		var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

		return await context.Set<TEntity>().CountAsync();
	}

	[OneTimeTearDown]
	public void RunAfterAnyTests()
	{
		//clears test db upon exit
		//_checkpoint.ResetAsync(connection).Wait();
		connection.Close();
	}
}


