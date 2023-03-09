using MediatR;
using Microsoft.EntityFrameworkCore;
using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.Exceptions;
using Tournament.Application.Contests.Queries.GetContests;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Tournament.Application.Common.Security;

namespace Tournament.Application.Contests.Queries.GetContest;

[Authorize]
public record GetContestQuery : IRequest<ContestBriefDto>
{
	public GetContestQuery(int id){
		ContestId=id;
	}
	public int ContestId { get; init; }
}

public class GetContestQueryHandler : IRequestHandler<GetContestQuery, ContestBriefDto?>
{
	private readonly IApplicationDbContext _context;
	private readonly IMapper _mapper;
	public GetContestQueryHandler(IApplicationDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public async Task<ContestBriefDto> Handle(GetContestQuery request, CancellationToken cancellationToken)
	{
		var contest=await _context.Contests.FindAsync(request.ContestId);
		if (contest==null){

			throw new NotFoundException (nameof(contest),request.ContestId);

		}

		return await  _context.Contests
			.Where(x => x.Id == request.ContestId)
			.ProjectTo<ContestBriefDto>(_mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();
	}
}

