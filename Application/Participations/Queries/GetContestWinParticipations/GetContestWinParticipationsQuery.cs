using MediatR;
using Microsoft.EntityFrameworkCore;
using Tournament.Application.Common.Security;
using Tournament.Application.Common.Interfaces;
using Tournament.Application.Accounts.Queries.GetAccountParticipations;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Tournament.Application.Participations.Queries.GetContestWinParticipations;

[Authorize]
public record GetContestWinParticipationsQuery : IRequest<List<ParticipationFullDto>>
{
	public GetContestWinParticipationsQuery(int id){
		ContestId=id;
	}
	public int ContestId { get; init; }
}

public class GetContestWinParticipationsQueryHandler : IRequestHandler<GetContestWinParticipationsQuery, List<ParticipationFullDto>>
{
	private readonly IApplicationDbContext _context;
	private readonly IMapper _mapper;
	public GetContestWinParticipationsQueryHandler(IApplicationDbContext context, IMapper mapper)
	{
		_context = context;
		 _mapper = mapper;
	}

	public async Task<List<ParticipationFullDto>> Handle(GetContestWinParticipationsQuery request, CancellationToken cancellationToken)
	{
		var parts=_context.Participations
			.Where(x => x.ContestId == request.ContestId && x.DrawnRank!=0)
			.ProjectTo<ParticipationFullDto>(_mapper.ConfigurationProvider);
		return await parts.ToListAsync();
	}
}

