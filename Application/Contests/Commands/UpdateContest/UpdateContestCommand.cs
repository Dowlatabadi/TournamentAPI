using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.Exceptions;
using MediatR;
using Tournament.Application.Common.Security;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.EntityFrameworkCore;
using FluentValidation.Results;
using Tournament.Domain.Entities;
using System;

namespace Tournament.Application.Contests.Commands.UpdateContest;

[Authorize]
public sealed record UpdateContestCommand : IRequest<int>
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public int ChannelId { get; init; }
    public int ContestId { get; init; }
    public bool IsActive { get; init; }
    public DateTime? Start { get; set; }
    public DateTime? Finish { get; set; }
    public DateTime? CalculateOn { get; set; }
    public bool WeightedDraw { get; init; }
    public bool WeightedReward { get; init; }
    public double Reward { get; init; }
    public int WinnersCapacity { get; init; }
    public int ParticipationCapacity { get; init; }
    public List<QuestionCreateDto> Questions { get; init; }
}
public sealed record QuestionCreateDto : IEquatable<QuestionCreateDto>
{
    public string? Title { get; init; }
    public int Order { get; init; }
    public IEnumerable<OptionUpdateDto> options { get; init; }
    public bool Equals(QuestionCreateDto? other)
    {
        if (other == null)
            return false;

        bool title_eq = (string.IsNullOrEmpty(Title) && string.IsNullOrEmpty(other.Title)) || (Title.Equals(other?.Title));
        bool options_eq = options.OrderBy(x => x.Title).SequenceEqual(other.options.OrderBy(x => x.Title));
        return title_eq && options_eq;
    }
}
public sealed record OptionUpdateDto : IEquatable<OptionUpdateDto>
{
    public string? Title { get; set; }
    public string? Text { get; set; }
    public int Order { get; set; }
    public bool Equals(OptionUpdateDto? other)
    {
        if (other == null)
            return false;

        bool title_eq = (string.IsNullOrEmpty(Title) && string.IsNullOrEmpty(other?.Title)) || (Title.Equals(other?.Title));
        bool text_eq = (string.IsNullOrEmpty(Text) && string.IsNullOrEmpty(other?.Text)) || (Text.Equals(other?.Text));
        return title_eq && text_eq;
    }
}
public class UpdateContestCommandHandler : IRequestHandler<UpdateContestCommand, int>
{
    private readonly IApplicationDbContext _context;
    public UpdateContestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<int> Handle(UpdateContestCommand request, CancellationToken cancellationToken)
    {
        var oldEntity = await _context.Contests
            .Include(x => x.Questions)
            .ThenInclude(x => x.Options)
            .FirstOrDefaultAsync(x => x.Id == request.ContestId);
        if (oldEntity == null)
        {

            throw new NotFoundException(nameof(oldEntity), request.ContestId);

        }

        //Compare questions and options
        var QOs_Changed = !oldEntity.Questions.Select(x => new QuestionCreateDto { Title = x.Title, options = x.Options.Select(y => new OptionUpdateDto { Title = y.Title, Text = y.Text }).OrderBy(t => t.Title) })
            .OrderBy(x => x).SequenceEqual(request.Questions.OrderBy(x => x));
        //(if has participations throw error "this contest has participations and you can't update the questions or options in this case") remove QOs and append
        var has_participations = _context.Participations.Any(x => x.ContestId == request.ContestId);

        if (QOs_Changed)
        {
            if (has_participations)
            {
                throw new ValidationException(new List<ValidationFailure> { new ValidationFailure() { ErrorCode = "881", PropertyName = "Contest", ErrorMessage = "This contest has participations and you can't update the questions or options." } });
            }
            _context.Options.RemoveRange(oldEntity.Questions.SelectMany(x => x.Options));
            _context.Questions.RemoveRange(oldEntity.Questions);
            oldEntity.Questions = request.Questions.Select(x => new Question { Title = x.Title, Options = x.options.Select(y => new Option { Title = y.Title, Text = y.Text }).ToList() }).ToList();
        }
        //else 
        //if QOs not modified update contest
        //cahnge on parameters nothing to do about Q O s


        //compare contest itself 
        //if modified ,then modify
        //else dont
        oldEntity.Title = request.Title;
        oldEntity.Description = request.Description;
        oldEntity.ChannelId = request.ChannelId;
        oldEntity.IsActive = request.IsActive;
        oldEntity.Start = request.Start;
        oldEntity.Finish = request.Finish;
        oldEntity.CalculateOn = request.CalculateOn;
        oldEntity.WeightedDraw = request.WeightedDraw;
        oldEntity.WeightedReward = request.WeightedReward;
        oldEntity.Reward = request.Reward;
        oldEntity.WinnersCapacity = request.WinnersCapacity;
        oldEntity.ParticipationCapacity = request.ParticipationCapacity;

        _context.Contests.Update(oldEntity);
        await _context.SaveChangesAsync(cancellationToken);
        return oldEntity.Id;
    }
}
