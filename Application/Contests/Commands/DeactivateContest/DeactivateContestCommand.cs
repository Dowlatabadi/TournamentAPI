using MediatR;
using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.Exceptions;
using AutoMapper;
using Tournament.Application.Common.Security;
using Microsoft.EntityFrameworkCore;

namespace Tournament.Application.Contests.Commands.DeactivateContestCommand;

[Authorize]
public record DeactivateContestCommand : IRequest<Unit>
{
	public DeactivateContestCommand(int id){
		ContestId=id;
	}
	public int ContestId { get; init; }
}

public class DeactivateContestCommandHandler : IRequestHandler<DeactivateContestCommand,Unit>
{
	private readonly IApplicationDbContext _context;
	private readonly IMapper _mapper;

	public DeactivateContestCommandHandler(IApplicationDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public async Task<Unit> Handle(DeactivateContestCommand request, CancellationToken cancellationToken)
	{
		var contest =await _context.Contests
            .Include(x => x.Participations)
            .ThenInclude(x => x.Answers)
            .Include(x => x.Questions)
            .ThenInclude(x => x.Options)
            .FirstOrDefaultAsync(x => x.Id == request.ContestId);
        if (contest==null){
			throw new NotFoundException (nameof(contest),request.ContestId);
		}
        _context.Answers.RemoveRange(contest.Participations.SelectMany(x => x.Answers));
        _context.Participations.RemoveRange(contest.Participations);

        contest.IsActive=false;
		await _context.SaveChangesAsync(cancellationToken);
		return Unit.Value;
	}
}

