using Tournament.Application.Options.Commands.CreateOption;
using Tournament.Application.Options.Queries.GetQuestionOptions;
using Tournament.Application.Questions.Queries.GetContestQuestions;
using Tournament.Application.Options.Commands.SetAsAnswerOption;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Tournament.WebAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class QuestionsController : ApiControllerBase
	{
		private readonly ILogger<QuestionsController> _logger;

		public QuestionsController(ILogger<QuestionsController> logger)
		{
			_logger = logger;
		}

		[SwaggerOperation(Summary = "Get a list of all options in a specified question by ID.")]
		[HttpGet("{id}/options")]
		public async Task<ActionResult<List<OptionDto>>> Get(int id)
		{
			var query=new GetQuestionOptionsQuery(id);
			return await Mediator.Send(query);
		}

		[SwaggerOperation(Summary = "Add a new option to specified question.")]
		[HttpPost("{id}/options")]
		public async Task<ActionResult<int>> Create(int id,CreateOptionCommand command)
		{
			if (id!=command.QuestionId){
				return BadRequest();
			}
			return await Mediator.Send(command);
		}

		[SwaggerOperation(Summary = "Sets an option as the answer of a question (resolve a single question specified by its Id and in case of resolving all qs of a contest, resolve the contest itself).")]
		[HttpPost("{id}/answer")]
		public async Task<ActionResult> Create(int id,SetAsAnswerOptionCommand command)
		{
			if (id!=command.QuestionId){
				return BadRequest();
			}
			await Mediator.Send(command);

			return NoContent();
		}
//		[HttpPost("{id}/answers")]
//		public async Task<ActionResult<int>> Create(int id,CreateOptionCommand command)
//		{
//			if (id!=command.QuestionId){
//				return BadRequest();
//			}
//			return await Mediator.Send(command);
//		}
	}
}

