using FluentValidation;
using Tournament.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Tournament.Domain.Entities;

namespace Tournament.Application.Participations.Commands.CreateParticipation;


public class CreateParticipationCommandValidator : AbstractValidator<CreateParticipationCommand>
{

    private readonly IApplicationDbContext _context;
    public CreateParticipationCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v)
            .MustAsync(NotParticipatedYet).WithMessage($"Account Already participated.");

        RuleFor(v => v)
            .MustAsync(NumberOfAnswersMatch).WithMessage($"Number of answers and Qs mismatch.");

        RuleFor(v => v)
            .MustAsync(HasFreeCapacity).WithMessage($"Participation capacity has no free room.");

        RuleFor(v => v)
    .MustAsync(NotDupQuestion).WithMessage($"Repetition in Answers of a Question.");
    }

    public async Task<bool> NotParticipatedYet(CreateParticipationCommand req, CancellationToken cancellationToken)
    {
        return !await _context.Participations.AnyAsync(x => x.AccountId == req.AccountId && x.ContestId == req.ContestId);
    }
    public async Task<bool> HasFreeCapacity(CreateParticipationCommand req, CancellationToken cancellationToken)
    {
        var contestCap = (await _context.Contests.Where(x => x.Id == req.ContestId).FirstOrDefaultAsync())?.ParticipationCapacity;
        if (contestCap != null)
        {
            if (contestCap == 0)
            { return true; }
            return (await _context.Participations.Where(x => x.ContestId == req.ContestId).CountAsync()) < contestCap;
        }
        return false;
    }
    public async Task<bool> NumberOfAnswersMatch(CreateParticipationCommand req, CancellationToken cancellationToken)
    {
        var questions =await _context.Questions.Where(x => x.ContestId == req.ContestId).ToListAsync();
        if (questions == null) return false;
        return questions.Count() == (req.OptionIds?.Count ?? 0);
    }

    public async Task<bool> NotDupQuestion(CreateParticipationCommand req, CancellationToken cancellationToken)
    {
        if (req.OptionIds == null) { return false; }
        var qs = _context.Questions.Where(x => x.ContestId == req.ContestId);
        if (!await qs.AnyAsync()) return false;
        var qids = await qs.Select(x => x.Id).ToListAsync();
        var options = _context.Options.Where(x => qids.Contains( x.QuestionId));
        if (!options.Any()) return false;
        return await options.Where(x => req.OptionIds.Contains(x.Id)).GroupBy(x => x.QuestionId).AllAsync(x => x.Count() == 1);
    }

}
