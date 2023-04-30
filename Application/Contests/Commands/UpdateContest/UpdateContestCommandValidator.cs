using FluentValidation;
using Tournament.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace Tournament.Application.Contests.Commands.UpdateContest;


public class UpdateContestCommandValidator : AbstractValidator<UpdateContestCommand>
{

    private readonly IApplicationDbContext _context;
    public UpdateContestCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Title)
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
            .NotEmpty();

        RuleFor(v => v.ChannelId)
            .NotEmpty().WithMessage("ChannelId is required.")
            .MustAsync(BeValidChannelId).WithMessage("The Specified ChannelId doesn't Exist.");

        RuleFor(v => v)
          .MustAsync(BeMinParticipated).WithMessage(x => $"Participations more than {x.ParticipationCapacity}.");

        RuleFor(v => new { v.CalculateOn, v.Finish })
            .Must(x => x.CalculateOn > x.Finish).WithMessage("Calculation time must be greater than Finish time.");

        RuleFor(v => new { v.Start, v.Finish })
    .Must(x => x.Finish > x.Start).WithMessage("Finish time must be greater than Start time.");

        RuleFor(v => v.Reward)
            .Must(x => x > 0)
            .When(x => x.WeightedReward == false)
            .WithMessage("Equal rewards is valid iff players don't spend (Reward>0 <=> WeightedReward==False)");

        RuleFor(v => (new { v.WinnersCapacity, v.ParticipationCapacity }))
            .Must(x => x.ParticipationCapacity >= x.WinnersCapacity).WithMessage("The Contest Participations cap can't be lower than wincap.");
    }

    public async Task<bool> BeValidChannelId(int chId, CancellationToken cancellationToken)
    {
        return await _context.Channels.AnyAsync(x => x.Id == chId);
    }
    public async Task<bool> BeMinParticipated(UpdateContestCommand CMD, CancellationToken cancellationToken)
    {
        if (CMD.ParticipationCapacity == 0)
        {
            return true;
        }
        var contest =await _context.Contests.FirstOrDefaultAsync(x => x.Id == CMD.ContestId);
        if (contest == null)
        {
            return true;
        }
        var parts = await _context.Participations.Where(x => x.ContestId == CMD.ContestId).CountAsync();
        if (parts > CMD.ParticipationCapacity)
        {
            return false;
        }
        return true;
    }

}
