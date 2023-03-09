using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Tournament.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger;

    public LoggingBehaviour(ILogger logger )
   { 
        _logger = logger;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        string userName = string.Empty;


        _logger.LogInformation("Tournament Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userName, userName, request);
    }
}
