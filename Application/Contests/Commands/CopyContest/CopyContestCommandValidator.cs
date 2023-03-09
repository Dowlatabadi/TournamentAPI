using FluentValidation;
using Tournament.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Tournament.Application.Contests.Commands.CopyContest;


public class CopyContestCommandValidator : AbstractValidator<CopyContestCommand>
{

	private readonly IApplicationDbContext _context;
	public CopyContestCommandValidator(IApplicationDbContext context)
	{
		_context=context;

		RuleFor(v=>v.ChannelId)
			.NotEmpty().WithMessage("ChannelId is required.")
			.MustAsync(BeValidChannelId).WithMessage("The Specified ChannelId doesn't Exist.");
	}

	public async Task<bool> BeValidChannelId(int chId, CancellationToken cancellationToken){
		return await _context.Channels.AnyAsync(x=>x.Id==chId);
	}
}
