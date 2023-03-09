using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tournament.Application.Common.Security;
namespace Tournament.Application.Options.Commands.SetAsAnswerOption;

[Authorize]
public record SetAsAnswerOptionCommand : IRequest<Unit>
{
    public int OptionId { get; init; }
    public int QuestionId { get; init; }
}
public class SetAsAnswerOptionCommandHandler : IRequestHandler<SetAsAnswerOptionCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    public SetAsAnswerOptionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Unit> Handle(SetAsAnswerOptionCommand request, CancellationToken cancellationToken)
    {
        var option = await _context.Options.FindAsync(request.OptionId);
		if (option==null) 
		{
			throw new NotFoundException (nameof(option),request.OptionId);
		}

		var allOptions=_context.Options.Include(x=>x.Question).Where(x=>x.QuestionId==request.QuestionId).ToList();
		if (!allOptions.Any(x=>x.Id==request.OptionId))
		{
			throw new Exception ($"Irrelevant option {request.OptionId}");
		}
		allOptions.ForEach(x=>x.IsAnswer=false);
        option.IsAnswer = true;

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
