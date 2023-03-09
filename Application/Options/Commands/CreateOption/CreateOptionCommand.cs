using Tournament.Application.Common.Interfaces;
using Tournament.Domain.Entities;
using MediatR;
using Tournament.Application.Common.Security;
namespace Tournament.Application.Options.Commands.CreateOption;

[Authorize]
public record CreateOptionCommand : IRequest<int>
{
    public string? Title { get; init; }
    public string? Text { get; init; }
    public bool IsAnswer { get; init; }=false;
    public int QuestionId { get; init; }
}
public class CreateOptionCommandHandler : IRequestHandler<CreateOptionCommand, int>
{
    private readonly IApplicationDbContext _context;
    public CreateOptionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<int> Handle(CreateOptionCommand request, CancellationToken cancellationToken)
    {
        var entity = new Option();
        entity.Title = request.Title;
        entity.Text = request.Text;
        entity.IsAnswer = request.IsAnswer;
        entity.QuestionId=request.QuestionId;
        _context.Options.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
