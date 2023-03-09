using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Tournament.Application.Common.Security;

namespace Tournament.Application.Channels.Queries.GetChannel;

[Authorize]
public record GetChannelQuery : IRequest<ChannelDto>
{
	public GetChannelQuery(int id){
		Id=id;
	}
	public int Id { get; init; }
}
public class GetChannelQueryHandler : IRequestHandler<GetChannelQuery,ChannelDto>
{
	private readonly IApplicationDbContext _context;
	private readonly IMapper _mapper;

	public GetChannelQueryHandler(IApplicationDbContext context,IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}
	public async Task<ChannelDto> Handle(GetChannelQuery request, CancellationToken cancellationToken)
	{
		var channel=await _context.Channels.FindAsync(request.Id);
		if (channel==null){

			throw new NotFoundException (nameof(channel),request.Id);

		}

		return await _context.Channels
			.Where(x=>x.Id==request.Id)
			.AsNoTracking()
			.ProjectTo<ChannelDto>(_mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();
	}
}
