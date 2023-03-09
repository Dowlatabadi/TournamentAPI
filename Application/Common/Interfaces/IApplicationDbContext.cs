using Microsoft.EntityFrameworkCore;
using Tournament.Domain.Entities;

namespace Tournament.Application.Common.Interfaces
{
	public interface IApplicationDbContext
	{
		DbSet<Option> Options { get; }
		DbSet<Answer> Answers { get; }
		DbSet<Contest> Contests { get; }
		DbSet<Channel> Channels { get; }
		DbSet<Question> Questions { get; }
		DbSet<Participation>  Participations { get; }

		Task<int> SaveChangesAsync(CancellationToken cancellationToken);
	}
}
