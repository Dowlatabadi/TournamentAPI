using Tournament.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using Tournament.Application.Channels.Queries.GetChannel;
using Tournament.Application.Common.Security;

namespace Tournament.Application.Channels.Queries.GetChannels;

[Authorize]
public record GetChannelsQuery : IRequest<List<ChannelDto>>;
public class GetChannelsQueryHandler : IRequestHandler<GetChannelsQuery,List<ChannelDto>>
{
	private readonly IApplicationDbContext _context;
	private readonly IMapper _mapper;
	public GetChannelsQueryHandler(IApplicationDbContext context,IMapper mapper)
	{
		_context = context;
		_mapper= mapper;
	}
	public async Task<List<ChannelDto>> Handle(GetChannelsQuery request, CancellationToken cancellationToken)
	{
		return await _context.Channels
			.ProjectTo<ChannelDto>(_mapper.ConfigurationProvider)
			.ToListAsync();
	}
}
