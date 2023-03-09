using Tournament.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Tournament.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Tournament.Application.Options.EventHandlers;

public class OptionisCorrectEventHandler : INotificationHandler<OptionisCorrectEvent>
{
	private readonly ILogger<OptionisCorrectEventHandler> _logger;
	private readonly IApplicationDbContext _context;

	public OptionisCorrectEventHandler(ILogger<OptionisCorrectEventHandler> logger,IApplicationDbContext context)
	{
		_logger=logger;
		_context=context;
	}

	public async Task Handle(OptionisCorrectEvent notification, CancellationToken cancellationToken){

		_logger.LogInformation("Tournament Domain Event: {e}", notification.GetType().Name);

		//prevent null Question upon option creation trigger [created and set to answer at once]
		var question=_context.Questions.Include(x=>x.Contest).Where(x=>x.Id==notification.Item.QuestionId).FirstOrDefault();
		if (!question.Resolved){

			_logger.LogInformation($"Question resolved {notification.Item.QuestionId}");
			question.Resolved=true;

		}
		else{

		_logger.LogInformation($"Question already resolved {notification.Item.QuestionId}");

		}

		//is contest resolved now?
		var AllQs=_context.Questions.Where(x=>x.ContestId==question.ContestId).ToList();
		if (AllQs.All(x=>x.Resolved)){

			_logger.LogInformation($"All questions({AllQs.Count}) resolved for contest {notification.Item.Question.Contest.Id}");
			question.Contest.Resolved=true;

		}
		else{
			question.Contest.Resolved=false;
		}
	}
}
