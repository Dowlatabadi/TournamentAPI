using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.Security;

namespace Tournament.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehaviour(ILogger logger,ICurrentUserService currentUserService)
   { 
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        string userName = _currentUserService.UserName ?? string.Empty;


        _logger.LogInformation("Tournament Request: {Name} {@UserName} {@Request}",
            requestName, userName, request);
    }
}
