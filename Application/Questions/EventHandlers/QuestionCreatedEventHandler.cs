using Tournament.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Tournament.Application.Common.Interfaces;

namespace Tournament.Application.Questions.EventHandlers;

public class QuestionCreatedEventHandler : INotificationHandler<QuestionCreatedEvent>
{
	private readonly ILogger<QuestionCreatedEventHandler> _logger;
	private readonly IApplicationDbContext _context;

	public QuestionCreatedEventHandler(ILogger<QuestionCreatedEventHandler> logger,IApplicationDbContext context)
	{
		_logger=logger;
		_context=context;
	}

	public async Task Handle(QuestionCreatedEvent notification, CancellationToken cancellationToken){

		_logger.LogInformation("Tournament Domain Event: {e}", notification.GetType().Name);

		var contest=_context.Contests.Find(notification.Item.ContestId);
		if (contest.Resolved){
			contest.Resolved=false;
		}

	}
}
