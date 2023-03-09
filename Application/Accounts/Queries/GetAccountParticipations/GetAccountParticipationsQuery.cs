using MediatR;
using Microsoft.EntityFrameworkCore;
using Tournament.Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Tournament.Application.Common.Security;

namespace Tournament.Application.Accounts.Queries.GetAccountParticipations;

[Authorize]
public record GetAccountParticipationsQuery : IRequest<List<ParticipationFullDto>>
{
	public GetAccountParticipationsQuery(string id){
		AccountId=id;
	}
	public string AccountId { get; init; }
}

public class GetAccountParticipationsQueryHandler : IRequestHandler<GetAccountParticipationsQuery, List<ParticipationFullDto>>
{
	private readonly IApplicationDbContext _context;
	private readonly IMapper _mapper;
	public GetAccountParticipationsQueryHandler(IApplicationDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public async Task<List<ParticipationFullDto>> Handle(GetAccountParticipationsQuery request, CancellationToken cancellationToken)
	{
		var parts=_context.Participations
			.Where(x => x.AccountId == request.AccountId).ProjectTo<ParticipationFullDto>(_mapper.ConfigurationProvider);
		return await parts.ToListAsync();
	}
}

