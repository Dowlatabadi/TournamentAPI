using FluentValidation;
using Tournament.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Tournament.Application.Contests.Commands.DeleteContest;


public class DrawContestCommandValidator : AbstractValidator<DeleteContestCommand>
{

	private readonly IApplicationDbContext _context;
    public DrawContestCommandValidator(IApplicationDbContext context)
    {
		_context=context;

		RuleFor(v=>v.ContestId)
			.MustAsync(ContestExists).WithMessage("The Specified Contest Id is not associated with a contest.");
    }

	public async Task<bool> ContestExists(int cId, CancellationToken cancellationToken){
		return await _context.Contests.AnyAsync(x=>x.Id==cId);
	}
}
