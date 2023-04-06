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
        private readonly ILogger<MessageConsumerService> _logger;
        private readonly IMQInfrastructure _MQInfrastructure;
        private readonly IMediator _mediator;

        public MessageConsumerService(ILogger<MessageConsumerService> logger, IMQInfrastructure mqInfrastructure)
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
                        _logger.LogInformation("Dup message received and dumped successfully: @message", CompeteM);
                    }
                    _logger.LogInformation("Message proccessed succesfully :@Message", CompeteM);
                    return true;
                }
                catch (NotFoundException ex)
                {
                    //TODO Queue in redeem Q
                    _logger.LogInformation("Queued back in redeem Queue: @message", CompeteM);
                    return true;
                }
                catch (ValidationException ex)
                {
                    //TODO Queue in redeem Q
                    if (ex.Message.Contains("Account Already participated"))
                    {
                        _logger.LogCritical("Account Already participated shouldn't have occured!!!: @message", CompeteM);
                    }
                    _logger.LogInformation("Queued back in redeem Queue: @message", CompeteM);
                    return true;
                }
                catch (AggregateException ex)
                {
                    if (ex.InnerExceptions.Any(x => x is ValidationException))
                    {
                        if (ex.InnerExceptions.Any(x => x.Message.Contains("Account Already participated")))
                        {
                            _logger.LogCritical("Account Already participated shouldn't have occured!!!: @message", CompeteM);
                        }
                        _logger.LogInformation("Queued back in redeem Queue: @message", CompeteM);
                        return true;
                    }
                    _logger.LogError($"Error during message proccess[REQUEUED]: @Message", CompeteM);
                    return false;
                }

                catch (Exception ex)
                {
                    _logger.LogError($"Error during message proccess[REQUEUED]: @Message", CompeteM);
                    return false;
                }
            }


        }
    }
}
