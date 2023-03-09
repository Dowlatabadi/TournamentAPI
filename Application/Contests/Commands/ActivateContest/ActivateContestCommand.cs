using MediatR;
using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.Exceptions;
using AutoMapper;
using Tournament.Application.Common.Security;

namespace Tournament.Application.Contests.Commands.ActivateContestCommand;

[Authorize]
public record ActivateContestCommand : IRequest<Unit>
{
	public ActivateContestCommand(int id){
		ContestId=id;
	}
	public int ContestId { get; init; }
}

public class ActivateContestCommandHandler : IRequestHandler<ActivateContestCommand,Unit>
{
	private readonly IApplicationDbContext _context;
	private readonly IMapper _mapper;

	public ActivateContestCommandHandler(IApplicationDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public async Task<Unit> Handle(ActivateContestCommand request, CancellationToken cancellationToken)
	{
		var contest=await _context.Contests.FindAsync(request.ContestId);
		if (contest==null){
			throw new NotFoundException (nameof(contest),request.ContestId);
		}
		contest.IsActive=true;
		await _context.SaveChangesAsync(cancellationToken);
		return Unit.Value;
	}
}

