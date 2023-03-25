using Tournament.Application.Common.Models;
using Tournament.Application.Contests.Commands.CreateContest;
using Tournament.Application.Contests.Commands.UpdateContest;
using Tournament.Application.Contests.Commands.DeleteContest;
using Tournament.Application.Contests.Commands.CopyContest;
using Tournament.Application.Contests.Commands.DrawContest;
using Tournament.Application.Contests.Commands.ActivateContestCommand;
using Tournament.Application.Contests.Commands.DeactivateContestCommand;
using Tournament.Application.Contests.Queries.GetContests;
using Tournament.Application.Contests.Queries.GetContest;
using Tournament.Application.Contests.Queries.GetStat;
using Tournament.Application.Participations.Queries.GetContestParticipations;
using Tournament.Application.Participations.Queries.GetContestWinParticipations;
using Tournament.Application.Participations.Commands.CreateParticipation;
using Tournament.Application.Questions.Queries.GetContestQuestions;
using Tournament.Application.Accounts.Queries.GetAccountParticipations;
using Tournament.Application.Questions.Commands.CreateQuestion;
using Tournament.Application.Contests.Commands.ResetDraw;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Tournament.WebAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ContestsController : ApiControllerBase
	{

		private readonly ILogger<ContestsController> _logger;

		public ContestsController(ILogger<ContestsController> logger)
		{
			_logger = logger;
		}

		[SwaggerOperation(Summary = "Get a specific contest by ID")]
		[HttpGet("{id}")]
		public async Task<ActionResult<ContestBriefDto>> Get(int id)
		{
			var query=new GetContestQuery(id);
			return await Mediator.Send(query);
		}

		[SwaggerOperation(Summary = "Get All Participations of a specific contest by ID")]
		[HttpGet("{id}/participations")]
		public async Task<ActionResult<PaginatedList<ParticipationFullDto>>> GetParticipations(int id,int PageNumber,int PageSize)
		{
			var query=new GetContestParticipationsQuery(id,PageNumber,PageSize);
			return await Mediator.Send(query);
		}
		

		[SwaggerOperation(Summary = "Get All Win Participations of a specific contest by ID")]
		[HttpGet("{id}/wins")]
		public async Task<ActionResult<List<ParticipationFullDto>>> GetWinParticipations(int id)
		{
			var query=new GetContestWinParticipationsQuery(id);
			return await Mediator.Send(query);
		}
		
		[SwaggerOperation(Summary = "Get All questions of a specific contest by ID")]
		[HttpGet("{id}/questions")]
		public async Task<ActionResult<List<QuestionDto>>> GetQuestions(int id)
		{
			var query=new GetContestQuestionsQuery(id);
			return await Mediator.Send(query);
		}

		[SwaggerOperation(Summary = "Get a list of all contests based on search parameters.")]
		[HttpGet(Name = "SearchContests")]
		public async Task<ActionResult<PaginatedList<ContestBriefDto>>> Search([FromQuery]GetContestsQuery query)
		{
			return await Mediator.Send(query);
		}

		[SwaggerOperation(Summary = "Get statistics of a specific contest by ID")]
        [HttpGet("{id}/stat")]
        public async Task<ActionResult<List<QuestionStatDto>>> GetStat(int id)
        {
			var query = new GetContestStatQuery(id);
            return await Mediator.Send(query);
        }

		[SwaggerOperation(Summary = "Clones a contest specific by ID")]
        [HttpPost]
		[Route("Clone")]
		public async Task<ActionResult<int>> Copy(CopyContestCommand command)
		{
			return await Mediator.Send(command);
		}

		[SwaggerOperation(Summary = "Creates a contest using defined parameters.")]
		[HttpPost]
		public async Task<ActionResult<int>> Create(CreateContestCommand command)
		{
			return await Mediator.Send(command);
		}

		[SwaggerOperation(Summary = "Updates a contest using defined parameters.")]
		[HttpPut("{id}")]
		public async Task<ActionResult<int>> Update(int id,UpdateContestCommand command)
		{
			if (id!=command.ContestId){
				return BadRequest();
			}

			await Mediator.Send(command);

			return NoContent();
		}

		[SwaggerOperation(Summary = "Deletes a contest")]
		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			DeleteContestCommand command=new DeleteContestCommand(id);
			await Mediator.Send(command);

			return NoContent();
		}

		[SwaggerOperation(Summary = "sets a contest to active state contest.")]
		[HttpPost("{id}/activate")]
		public async Task<ActionResult> Activate(int id)
		{
			var command=new ActivateContestCommand(id);
			await Mediator.Send(command);

			return NoContent();
		}

		[SwaggerOperation(Summary = "draws winners of a contest according to redefined win parameters.")]
		[HttpPost("{id}/draw")]
		public async Task<ActionResult> Draw(int id)
		{
			var command=new DrawContestCommand(id);
			await Mediator.Send(command);

			return NoContent();
		}

		[SwaggerOperation(Summary = "clears the draw resutls(drawn rank, reward, ..).")]
		[HttpPost("{id}/resetdraw")]
		public async Task<ActionResult> Resetdraw(int id)
		{
			var command=new ResetDrawCommand(id);
			await Mediator.Send(command);

			return NoContent();
		}

		[SwaggerOperation(Summary = "deactivates a contest.")]
		[HttpPost("{id}/deactivate")]
		public async Task<ActionResult> Deactivate(int id)
		{
			var command=new DeactivateContestCommand(id);
			await Mediator.Send(command);

			return NoContent();
		}

		[SwaggerOperation(Summary = "Add a question to a contest specific by ID.")]
		[HttpPost("{id}/questions")]
		public async Task<ActionResult<int>> Create(int id,CreateQuestionCommand command)
		{
			if (id!=command.ContestId){
				return BadRequest();
			}

			return await Mediator.Send(command);
		}

		[SwaggerOperation(Summary = "participate in contest specified by ID and set supplied options as answers.")]
		[HttpPost("{id}/participations")]
		public async Task<ActionResult<int>> Create(int id,CreateParticipationCommand command)
		{
			if (id!=command.ContestId){
				return BadRequest();
			}

			return await Mediator.Send(command);
		}
	}
}
