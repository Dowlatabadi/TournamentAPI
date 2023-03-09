using Tournament.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Tournament.Application.Common.Exceptions;
using Tournament.Domain.Entities;
using MediatR;
using Tournament.Application.Common.Security;
namespace Tournament.Application.Contests.Commands.CopyContest;

[Authorize]
public record CopyContestCommand : IRequest<int>
{
	public int ChannelId { get; init; }
	public int ContestId {get; init;} 
}
public class CopyContestCommandHandler : IRequestHandler<CopyContestCommand, int>
{
	private readonly IApplicationDbContext _context;
	public CopyContestCommandHandler(IApplicationDbContext context)
	{
		_context = context;
	}
	public async Task<int> Handle(CopyContestCommand request, CancellationToken cancellationToken)
	{
		var contest=await _context.Contests.Include(x=>x.Questions)
			.ThenInclude(x=>x.Options).Where(x=>x.Id==request.ContestId)
			.FirstOrDefaultAsync();
		if (contest==null){
			throw new NotFoundException (nameof(contest),request.ContestId);
		}

		var Qs=contest.Questions.Select(x=>new Question{
				Title=x.Title,
				Options=x.Options.Select(y=>new Option{Title=y.Title,Text=y.Text}).ToList()
				}).ToList();

		Contest contest1=new Contest() {
			Title = contest.Title,
				  ChannelId=request.ChannelId,
				  WeightedDraw=contest.WeightedDraw,
				  WeightedReward=contest.WeightedReward,
				  Reward=contest.Reward,
				  WinnersCapacity=contest.WinnersCapacity,
				  Questions=Qs
		};

		_context.Contests.Add(contest1);
		await _context.SaveChangesAsync(cancellationToken);
		return contest1.Id;
	}
}
