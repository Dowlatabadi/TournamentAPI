using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tournament.Application.Common.Exceptions;
using Tournament.Application.Participations.Commands.CreateParticipation;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Tournament.Application.Common.MQ
{
    public class MessageConsumerService : IMessageConsumerService
    {
        private readonly ILogger _logger;
        private readonly IMQInfrastructure _MQInfrastructure;
        private readonly IMediator _mediator;

        public MessageConsumerService(ILogger logger, IMQInfrastructure mqInfrastructure)
        {
            _logger = logger;
            _MQInfrastructure = mqInfrastructure;
            _MQInfrastructure.ConfigureConsumer((x, scopeFactory) => Consume(x, scopeFactory));
        }
        private bool Consume(byte[] receivedBytes, IServiceScopeFactory serviceScopeFactory)
        {
            CompeteMQMessage? CompeteM = null;
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<ISender>();
                try
                {
                    CompeteM = JsonSerializer.Deserialize<CompeteMQMessage>(Encoding.UTF8.GetString(receivedBytes));
                    try
                    {
                        CreateParticipationCommand CPCommand = new CreateParticipationCommand
                        {
                            AccountId = CompeteM.AccountId,
                            ContestId = CompeteM.ContestId,
                            OptionIds = CompeteM.Options,
                            Spent = CompeteM.Spent,
                        };
                        int result = mediator.Send(CPCommand).Result;
                        if (result == 0)
                        {
                            //result==0 according to the handler means already participated
                            _logger.LogInformation("[MQ] Dup message received and dumped successfully: {@message}", CompeteM);
                        }
                        _logger.LogInformation("[MQ] Message proccessed succesfully :{@Message}", CompeteM);
                        return true;
                    }
                    catch (NotFoundException ex)
                    {
                        _logger.LogError(ex, "[MQ] Queued back in redeem Queue: {@message}", CompeteM);
                        var topublish = new { reason = "contest might be deleted/drawn/inactivated", contestId = CompeteM.ContestId, accountId = CompeteM.AccountId, spent = CompeteM.Spent };
                        var jsonString = JsonSerializer.Serialize(topublish);
                        _MQInfrastructure.PublishMessage(jsonString);
                        return true;
                    }
                    catch (ValidationException ex)
                    {  //capacity and already are handled in app 1 and should not be happened
                        //malformed input(options mistmach) is not handled in app 1 and should be redeemed
                        if (ex.Message.Contains("Account Already participated"))
                        {
                            _logger.LogCritical("[MQ] Account Already participated shouldn't have occured!!!: {@message}", CompeteM);
                        }
                        _logger.LogError(ex, "[MQ] Queued back in redeem Queue: {@message} ", CompeteM );
                        var topublish = new { reason = $"command couldn't be validate {ex.Message ?? ""}", contestId = CompeteM.ContestId, accountId = CompeteM.AccountId, spent = CompeteM.Spent };
                        var jsonString = JsonSerializer.Serialize(topublish);
                        _MQInfrastructure.PublishMessage(jsonString);
                        return true;
                    }
                    catch (AggregateException ex)
                    {
                        //if agg error is validation error should be redeemed
                        if (ex.InnerExceptions.Any(x => x is ValidationException))
                        {
                            if (ex.InnerExceptions.Any(x => x.Message.Contains("Account Already participated")))
                            {
                                _logger.LogCritical("[MQ] Account Already participated shouldn't have occured!!!: {@message}", CompeteM);
                            }
                            _logger.LogError(ex, "[MQ] Queued back in redeem Queue: {@message}"+$" Reason: {String.Join(',', ex.InnerExceptions?.Select(x => x.Message))}",  CompeteM);
                            var topublish = new { reason = $"command had agg error {ex.InnerExceptions?.FirstOrDefault()?.Message ?? ex.Message}", contestId = CompeteM.ContestId, accountId = CompeteM.AccountId, spent = CompeteM.Spent };
                            var jsonString = JsonSerializer.Serialize(topublish);
                            _MQInfrastructure.PublishMessage(jsonString);
                            return true;
                        }
                        //if agg error is not validation it must be checked
                        _logger.LogError("[MQ] Error during message proccess[REQUEUED]: {@Message}", CompeteM);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        //something went wrong it must be checked
                        _logger.LogError(ex,"[MQ] Error during message proccess[REQUEUED]: {@Message}", CompeteM);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,"[MQ] Unable to deserialize message!! it is ignored for good");
                    return true;
                }
            }


        }
    }
}
