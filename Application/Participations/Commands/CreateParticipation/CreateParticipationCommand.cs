using Microsoft.EntityFrameworkCore;
using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.Exceptions;
using Tournament.Domain.Entities;
using MediatR;
using Tournament.Application.Common.Security;
namespace Tournament.Application.Participations.Commands.CreateParticipation;

[Authorize]
public record CreateParticipationCommand : IRequest<int>
{
	public int ContestId { get; init; }
	public string AccountId { get; init; }
	public double Spent { get; init; }
	public List<int> OptionIds { get; init; }
}
public class CreateParticipationCommandHandler : IRequestHandler<CreateParticipationCommand, int>
{
	private readonly IApplicationDbContext _context;
	public CreateParticipationCommandHandler(IApplicationDbContext context)
	{
		_context = context;
	}
	public async Task<int> Handle(CreateParticipationCommand request, CancellationToken cancellationToken)
	{
		Contest contest=await _context.Contests.Include(x=>x.Questions).ThenInclude(x=>x.Options).FirstOrDefaultAsync(x=>x.Id==request.ContestId);
		if (contest== null){
			throw new NotFoundException (nameof(contest),request.ContestId);
		}

		var entity = new Participation{
			AccountId=request.AccountId,
				ContestId=request.ContestId,
				Spent=request.Spent,
				Answers=request.OptionIds.Select(x=>new Answer {OptionId=x}).ToList()
		};

		_context.Participations.Add(entity);
		await _context.SaveChangesAsync(cancellationToken);
		return entity.Id;
	}
}
