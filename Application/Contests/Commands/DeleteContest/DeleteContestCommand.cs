using MediatR;
using Microsoft.EntityFrameworkCore;
using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.Exceptions;
using AutoMapper;
using Tournament.Application.Common.Security;
using System.Diagnostics;

namespace Tournament.Application.Contests.Commands.DeleteContest;

[Authorize]
public record DeleteContestCommand(int ContestId) : IRequest;

public class DeleteContestCommandHandler : IRequestHandler<DeleteContestCommand>
{
	private readonly IApplicationDbContext _context;
	private readonly IMapper _mapper;

	public DeleteContestCommandHandler(IApplicationDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public async Task<Unit> Handle(DeleteContestCommand request, CancellationToken cancellationToken)
	{
      
        //remove answers, participations, options, questions and then the contest itself
        var contest=await _context.Contests
			.Include(x=>x.Participations)
			.ThenInclude(x=>x.Answers)
			.Include(x=>x.Questions)
			.ThenInclude(x=>x.Options)
			.FirstOrDefaultAsync(x=>x.Id==request.ContestId);
		if (contest==null){
			throw new NotFoundException (nameof(contest),request.ContestId);
		}

        //remove answers, participations, options, questions and then the contest itself
        _context.Answers.RemoveRange(contest.Participations.SelectMany(x => x.Answers));
        _context.Options.RemoveRange(contest.Questions.SelectMany(x => x.Options));
        _context.Participations.RemoveRange(contest.Participations);
        _context.Questions.RemoveRange(contest.Questions);

        _context.Contests.Remove(contest);
		await _context.SaveChangesAsync(cancellationToken);
		return Unit.Value;
	}
}

