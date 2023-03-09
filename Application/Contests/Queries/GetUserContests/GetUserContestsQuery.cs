using MediatR;
using Microsoft.EntityFrameworkCore;
using Tournament.Application.Common.Models;
using Tournament.Application.Common.Interfaces;
using Tournament.Application.Contests.Queries.GetContests;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Tournament.Application.Common.Security;

namespace Tournament.Application.Contests.Queries.GetUserContests;

	[Authorize]
public record GetUserContestsQuery : IRequest<PaginatedList<ContestBriefDto>>
{
	public string AccountId { get; init; }
	public int PageNumber {get; init; } =1;
	public int PageSize {get; init; } =10;
}

public class GetUserContestsQueryHandler : IRequestHandler<GetUserContestsQuery, PaginatedList<ContestBriefDto?>>
{
	private readonly IApplicationDbContext _context;
	private readonly IMapper _mapper;
	public GetUserContestsQueryHandler(IApplicationDbContext context, IMapper mapper)
	{
		_context = context;
		 _mapper = mapper;
	}

	public async Task<PaginatedList<ContestBriefDto>> Handle(GetUserContestsQuery request, CancellationToken cancellationToken)
	{
		var contests =  _context.Participations
			.Where(x => x.AccountId == request.AccountId)
			.OrderBy(x => x.Id)
			.ProjectTo<ContestBriefDto>(_mapper.ConfigurationProvider);
		return await PaginatedList<ContestBriefDto>.CreateAsync(contests.AsNoTracking(), request.PageNumber, request.PageSize);
	}
}

