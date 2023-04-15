using FluentValidation;
using Tournament.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Tournament.Application.Contests.Commands.DrawContest;


public class DrawContestCommandValidator : AbstractValidator<DrawContestCommand>
{

    private readonly IApplicationDbContext _context;
    public DrawContestCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.ContestId)
            .MustAsync(BeResolvedContest).WithMessage("The Specified Contest is not resolved yet.");

        RuleFor(v => v.ContestId)
            .MustAsync(HasPlayer).WithMessage("The Specified Contest has no players.");

        RuleFor(v => v.ContestId)
            .MustAsync(IsActive).WithMessage("The Specified Contest has been deactivated.");

    }

    public async Task<bool> BeResolvedContest(int cId, CancellationToken cancellationToken)
    {
        return (await _context.Contests.FindAsync(cId)).Resolved;
    }

    public async Task<bool> HasPlayer(int cId, CancellationToken cancellationToken)
    {
        return await _context.Participations.Where(x => x.ContestId == cId).AnyAsync();
    }
    public async Task<bool> IsActive(int cId, CancellationToken cancellationToken)
    {
        return (await _context.Contests.FindAsync(cId)).IsActive;
    }
}
