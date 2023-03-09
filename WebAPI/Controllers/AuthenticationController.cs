using Tournament.Application.Auth.Queries.Authenticate;
using Microsoft.AspNetCore.Mvc;
using Tournament.WebAPI.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace Tournament.WebAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AuthenticationController : ApiControllerBase
	{
		private readonly ILogger<AuthenticationController> _logger;
		private readonly IJwtUtils _jwtutils;

		public AuthenticationController(ILogger<AuthenticationController> logger,IJwtUtils jwtutils)
		{
			_logger = logger;
			_jwtutils=jwtutils;
		}

		[SwaggerOperation(Summary = "Get Authentication Token")]
		[HttpPost("login")]
		public async Task<ActionResult<string>>  Login(AuthenticateQuery query)
		{
			if ( await Mediator.Send(query)){
				return _jwtutils.GenerateToken(query.Username);
			}
			return NoContent();
		}

	}
}

