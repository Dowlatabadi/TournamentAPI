using Tournament.Application.Common.Interfaces;
using Tournament.Domain.Entities;
using MediatR;
using Tournament.Application.Common.Security;
namespace Tournament.Application.Contests.Commands.CreateContest;

[Authorize]
public record CreateContestCommand : IRequest<int>
{
	public string? Title { get; init; }
	public string? Description { get; init; }
	public int ChannelId { get; init; }
	public bool IsActive {get; init;}
	public DateTime? Start { get; set; }
	public DateTime? Finish { get; set; }
	public bool WeightedDraw {get; init;}
	public bool WeightedReward {get; init;} 
	public double Reward {get; init;} 
	public int WinnersCapacity {get; init;}
	public int ParticipationCapacity { get; init; }
}
public class CreateContestCommandHandler : IRequestHandler<CreateContestCommand, int>
{
	private readonly IApplicationDbContext _context;
	public CreateContestCommandHandler(IApplicationDbContext context)
	{
		_context = context;
	}
	public async Task<int> Handle(CreateContestCommand request, CancellationToken cancellationToken)
	{
		var entity = new Contest();
		entity.Title = request.Title;
		entity.Description = request.Description;
		entity.ChannelId=request.ChannelId;
		entity.IsActive=request.IsActive;
		entity.Start=request.Start;
		entity.Finish=request.Finish;
		entity.WeightedDraw = request.WeightedDraw;
		entity.WeightedReward=request.WeightedReward;
		entity.Reward=request.Reward;
		entity.WinnersCapacity=request.WinnersCapacity;
		entity.ParticipationCapacity = request.ParticipationCapacity;

		_context.Contests.Add(entity);
		await _context.SaveChangesAsync(cancellationToken);
		return entity.Id;
	}
}
