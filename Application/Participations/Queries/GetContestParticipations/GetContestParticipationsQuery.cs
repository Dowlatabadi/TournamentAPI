using MediatR;
using Tournament.Application.Common.Mappings;
using Microsoft.EntityFrameworkCore;
using Tournament.Application.Common.Security;
using Tournament.Application.Common.Models;
using Tournament.Application.Common.Interfaces;
using Tournament.Application.Accounts.Queries.GetAccountParticipations;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Tournament.Application.Participations.Queries.GetContestParticipations;

[Authorize]
public record GetContestParticipationsQuery : IRequest<PaginatedList<ParticipationFullDto>>
{
	public GetContestParticipationsQuery(int Id,int pageNumber,int pageSize){
		ContestId=Id;
		PageNumber=pageNumber;
		PageSize=pageSize;
	}
	internal int ContestId { get; init; }=1;
	public int PageNumber { get; init; } = 1;
	public int PageSize { get; init; } = 10;
}

public class GetContestParticipationsQueryHandler : IRequestHandler<GetContestParticipationsQuery, PaginatedList<ParticipationFullDto>>
{
	private readonly IApplicationDbContext _context;
	private readonly IMapper _mapper;
	public GetContestParticipationsQueryHandler(IApplicationDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public async Task<PaginatedList<ParticipationFullDto>> Handle(GetContestParticipationsQuery request, CancellationToken cancellationToken)
	{
		var parts=_context.Participations
			.Where(x => x.ContestId == request.ContestId)
			.ProjectTo<ParticipationFullDto>(_mapper.ConfigurationProvider) ;

		return await parts.OrderBy(x => x.Id).PaginatedListAsync<ParticipationFullDto>(request.PageNumber == 0 ? 1 : request.PageNumber, request.PageSize == 0 ? 10 : request.PageSize);
	}
}

