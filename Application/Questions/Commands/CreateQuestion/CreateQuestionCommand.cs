using Tournament.Application.Common.Interfaces;
using Tournament.Domain.Entities;
using Tournament.Domain.Events;
using MediatR;
using Tournament.Application.Common.Security;
namespace Tournament.Application.Questions.Commands.CreateQuestion;

[Authorize]
public record CreateQuestionCommand : IRequest<int>
{
	public string? Title { get; init; }
	public int ContestId { get; init; }
    public int Order { get; set; }
}
public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, int>
{
	private readonly IApplicationDbContext _context;
	public CreateQuestionCommandHandler(IApplicationDbContext context)
	{
		_context = context;
	}
	public async Task<int> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
	{
		var entity = new Question();
		entity.Title = request.Title;
		entity.ContestId = request.ContestId;
		entity.Order = request.Order;

		entity.AddDomainEvent(new QuestionCreatedEvent(entity));
		_context.Questions.Add(entity);
		await _context.SaveChangesAsync(cancellationToken);
		return entity.Id;
	}
}

