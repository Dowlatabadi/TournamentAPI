using Tournament.Application.Accounts.Queries.GetAccountParticipations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Tournament.WebAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AccountController : ApiControllerBase
	{
		private readonly ILogger<AccountController> _logger;

		public AccountController(ILogger<AccountController> logger)
		{
			_logger = logger;
		}

		[SwaggerOperation(Summary = "Get all participations of a specific participant by his ID")]
		[HttpGet("{id}/participations")]
		public async Task<ActionResult<List<ParticipationFullDto>>> Get(string id)
		{
			var query=new GetAccountParticipationsQuery(id);
			return await Mediator.Send(query);
		}

	}
}

