using Tournament.Domain.Entities;
using Tournament.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Tournament.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
	private readonly ILogger<ApplicationDbContextInitialiser> _logger;
	private readonly ApplicationDbContext _context;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;

	public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
	{
		_logger = logger;
		_context = context;
		_userManager = userManager;
		_roleManager = roleManager;
	}

	public async Task InitialiseAsync()
	{
		try
		{
			if (_context.Database.IsSqlServer()) 
			{
				await _context.Database.MigrateAsync();
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while initialising the database.");
			throw;
		}
	}

	public async Task SeedAsync()
	{
		try
		{
			await TrySeedAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while seeding the database.");
			throw;
		}
	}

	public async Task TrySeedUsersAsync()
	{
		// Default roles
		var administratorRole = new IdentityRole("Administrator");

		if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
		{
			await _roleManager.CreateAsync(administratorRole);
		}

		// Default users
		var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

		if (_userManager.Users.All(u => u.UserName != administrator.UserName))
		{
			await _userManager.CreateAsync(administrator, "Administrator1!");
			await _userManager.AddToRolesAsync(administrator, new string[] { administratorRole.Name });
		}
	}

	public async Task TrySeedAsync()
	{

		// Default data
		// Seed, if necessary
		if (!_context.Channels.Any())
		{
            _context.Channels.Add(
              new Channel
              {
                  Title = "Channel title 1",
                  Contests ={
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
            _context.SaveChanges();
            _context.Answers.AddRange(new List<Answer> { new Answer { OptionId = 1, ParticipationId = 1 }, new Answer { OptionId = 3, ParticipationId = 1 }, new Answer { OptionId = 5, ParticipationId = 1 }, new Answer { OptionId = 7, ParticipationId = 1 } });
            _context.Answers.AddRange(new List<Answer> { new Answer { OptionId = 2, ParticipationId = 2 }, new Answer { OptionId = 3, ParticipationId = 2 }, new Answer { OptionId = 5, ParticipationId = 2 }, new Answer { OptionId = 7, ParticipationId = 2 } });
            _context.SaveChanges();
        }
	}
}
