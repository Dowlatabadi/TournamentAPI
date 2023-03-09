using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.Security;
using Tournament.Domain.Entities;
using MediatR;

namespace Tournament.Application.Channels.Commands.CreateChannel;

[Authorize]
public record CreateChannelCommand : IRequest<int>
{
    public string? Title { get; init; }
}
public class CreateChannelCommandHandler : IRequestHandler<CreateChannelCommand, int>
{
    private readonly IApplicationDbContext _context;
    public CreateChannelCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<int> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
    {
        var entity = new Channel();
        entity.Title = request.Title;
        _context.Channels.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
