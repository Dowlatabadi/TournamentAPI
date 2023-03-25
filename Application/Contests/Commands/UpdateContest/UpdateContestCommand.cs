using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.Exceptions;
using MediatR;
using Tournament.Application.Common.Security;
namespace Tournament.Application.Contests.Commands.UpdateContest;

[Authorize]
public record UpdateContestCommand : IRequest<int>
{
	public string? Title { get; init; }
	public string? Description { get; init; }
	public int ChannelId { get; init; }
	public int ContestId { get; init; }
	public bool IsActive {get; init;}
	public DateTime? Start { get; set; }
	public DateTime? Finish { get; set; }
	public bool WeightedDraw {get; init;}
	public bool WeightedReward {get; init;} 
	public double Reward {get; init;} 
	public int WinnersCapacity {get; init;}
	public int ParticipationCapacity { get; init; }
}
public class UpdateContestCommandHandler : IRequestHandler<UpdateContestCommand, int>
{
	private readonly IApplicationDbContext _context;
	public UpdateContestCommandHandler(IApplicationDbContext context)
	{
		_context = context;
	}
	public async Task<int> Handle(UpdateContestCommand request, CancellationToken cancellationToken)
	{
		var entity=await _context.Contests.FindAsync(request.ContestId);
		if (entity==null){

			throw new NotFoundException (nameof(entity),request.ContestId);

		}
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
