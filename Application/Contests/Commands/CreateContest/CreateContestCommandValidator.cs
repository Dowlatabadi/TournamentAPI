using FluentValidation;
using Tournament.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Tournament.Application.Contests.Commands.CreateContest;


public class CreateContestCommandValidator : AbstractValidator<CreateContestCommand>
{

	private readonly IApplicationDbContext _context;
    public CreateContestCommandValidator(IApplicationDbContext context)
    {
		_context=context;

		RuleFor(v=>v.Title)
			.MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
			.NotEmpty();

		RuleFor(v=>v.ChannelId)
			.NotEmpty().WithMessage("ChannelId is required.")
			.MustAsync(BeValidChannelId).WithMessage("The Specified ChannelId doesn't Exist.");

        RuleFor(v =>(new { v.WinnersCapacity,v.ParticipationCapacity }))
            .Must(x=>x.ParticipationCapacity>=x.WinnersCapacity).WithMessage("The Contest Participations cap can't be lower than wincap.");
    }

	public async Task<bool> BeValidChannelId(int chId, CancellationToken cancellationToken){
		return await _context.Channels.AnyAsync(x=>x.Id==chId);
	}
}
