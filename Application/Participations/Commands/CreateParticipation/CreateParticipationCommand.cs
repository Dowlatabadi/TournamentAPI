using Microsoft.EntityFrameworkCore;
using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.Exceptions;
using Tournament.Domain.Entities;
using MediatR;
using Tournament.Application.Common.Security;
using AutoMapper;

namespace Tournament.Application.Participations.Commands.CreateParticipation;

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
    //should only happen when consumed from Q, direct call should never reach here otherwise cost can't be redeemed
    public async Task<int> Handle(CreateParticipationCommand request, CancellationToken cancellationToken)
	{
		var iddel=_context.GetHashCode();
		var contest=await _context.Contests.Include(x=>x.Questions).ThenInclude(x=>x.Options).FirstOrDefaultAsync(x=>x.Id==request.ContestId);

		if (contest== null){
			//results in redeem
			throw new NotFoundException (nameof(contest),request.ContestId);
		}

        if (contest.Participations.Any(x=>x.DrawnRank>0))
        {
            //results in redeem
            throw new NotFoundException(nameof(contest), request.ContestId);
        }
        if (!contest.IsActive)
        {
            //results in redeem
            throw new NotFoundException(nameof(contest), request.ContestId);
        }
        var participated = await _context.Participations.FirstOrDefaultAsync(x => x.AccountId == request.AccountId && x.ContestId==request.ContestId);
        if (participated != null)
        {
			//means already participated and does nothing (further chekings should have been done before sending to Q by the producer)
			return 0;
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
