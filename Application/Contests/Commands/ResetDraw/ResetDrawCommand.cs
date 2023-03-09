using MediatR;
using Microsoft.EntityFrameworkCore;
using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.Exceptions;
using AutoMapper;
using Tournament.Application.Common.Security;

namespace Tournament.Application.Contests.Commands.ResetDraw;

[Authorize]
public record ResetDrawCommand : IRequest<Unit>
{
	public ResetDrawCommand(int id){
		ContestId=id;
	}
	public int ContestId { get; init; }
}

public class ResetDrawCommandHandler : IRequestHandler<ResetDrawCommand,Unit>
{
	private readonly IApplicationDbContext _context;
	private readonly IMapper _mapper;

	public ResetDrawCommandHandler(IApplicationDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public async Task<Unit> Handle(ResetDrawCommand request, CancellationToken cancellationToken)
	{
		var contest=await _context.Contests.FindAsync(request.ContestId);
		if (contest==null){
			throw new NotFoundException (nameof(contest),request.ContestId);
		}

		//preapare and count number of participations
		var Participations=await _context.Participations.Where(x=>x.ContestId==request.ContestId).ToListAsync();
		//reset previous draw
		foreach (var p in Participations){
			p.Reward=0;
			p.DrawnRank=0;
		}

		await _context.SaveChangesAsync(cancellationToken);
		return Unit.Value;
	}
}

