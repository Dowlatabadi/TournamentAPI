using MediatR;
using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.Mappings;
using Tournament.Application.Common.Extensions;
using Tournament.Application.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Tournament.Application.Common.Security;

namespace Tournament.Application.Contests.Queries.GetContests;

[Authorize]
public record GetContestsQuery : IRequest<PaginatedList<ContestBriefDto>>
{
    public int? ChannelId { get; init; }
    public string? ChannelTitle { get; init; }
    public string? AccountId { get; init; }
    public RangeSearch<DateTime?>? Start1 { get; init; }
    public RangeSearch<DateTime?>? Start2 { get; init; }
    public RangeSearch<DateTime?>? Finish1 { get; init; }
    public RangeSearch<DateTime?>? Finish2 { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public bool? IsActive { get; init; }
    public bool? Resolved { get; init; }
    public bool? WeightedDraw { get; init; }
    public bool? WeightedReward { get; init; }
}

public class GetContestsQueryHandler : IRequestHandler<GetContestsQuery, PaginatedList<ContestBriefDto?>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetContestsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ContestBriefDto>> Handle(GetContestsQuery request, CancellationToken cancellationToken)
    {
        var contests_init = _context.Contests
            .Where(x => request.ChannelId == null || x.ChannelId == request.ChannelId)
           .Where(x => string.IsNullOrEmpty(request.ChannelTitle) || x.Channel.Title.Equals(request.ChannelTitle))
            .WhereRangeSearch(x => x.Start, request.Start1)
            .WhereRangeSearch(x => x.Start, request.Start2)
            .WhereRangeSearch(x => x.Finish, request.Finish1)
            .WhereRangeSearch(x => x.Finish, request.Finish2)
            .WhereRangeSearch(x => x.IsActive, new RangeSearch<bool?>(SearchOperator.EqualTo, request.IsActive))
            .WhereRangeSearch(x => x.Resolved, new RangeSearch<bool?>(SearchOperator.EqualTo, request.Resolved))
            .WhereRangeSearch(x => x.WeightedReward, new RangeSearch<bool?>(SearchOperator.EqualTo, request.WeightedReward))
            .WhereRangeSearch(x => x.WeightedDraw, new RangeSearch<bool?>(SearchOperator.EqualTo, request.WeightedDraw));

        if (request.AccountId != null && request.AccountId?.Length > 3)
        {
            contests_init = contests_init
                .Where(x => x.Participations.Any(y => y.AccountId == request.AccountId));
        }

        var contests = contests_init.ProjectTo<ContestBriefDto>(_mapper.ConfigurationProvider);

        return await contests.OrderBy(x => x.Id).PaginatedListAsync<ContestBriefDto>(request.PageNumber == 0 ? 1 : request.PageNumber, request.PageSize == 0 ? 10 : request.PageSize);

    }
}

