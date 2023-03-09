using Tournament.Application.Channels.Queries.GetChannels;
using Tournament.Application.Channels.Queries.GetChannel;
using Tournament.Application.Channels.Commands.CreateChannel;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Tournament.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChannelsController : ApiControllerBase
    {

        private readonly ILogger<ChannelsController> _logger;

        public ChannelsController(ILogger<ChannelsController> logger)
        {
            _logger = logger;
        }

		[SwaggerOperation(Summary = "Get Channel information by its ID")]
		[HttpGet("{id}")]
        public async Task<ActionResult<ChannelDto>> Get(int id)
        {
			var query= new GetChannelQuery(id);
			return await Mediator.Send(query);
        }

		[SwaggerOperation(Summary = "Get all channels")]
		[HttpGet]
        public async Task<ActionResult<IList<ChannelDto>>> Get()
        {
			var query= new GetChannelsQuery();
			return await Mediator.Send(query);
        }

		[SwaggerOperation(Summary = "Create a channel")]
		[HttpPost]
        public async Task<ActionResult<int>> Create(CreateChannelCommand command)
        {
			return await Mediator.Send(command);
        }

    }
}
