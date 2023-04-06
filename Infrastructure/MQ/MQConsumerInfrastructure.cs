using Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.MQ
{
    public class MQConsumerInfrastructure : IMQInfrastructure
    {
        private readonly MQConsumerOptions _options;
        private readonly IModel channel;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _ServiceScopeFactory;
        public MQConsumerInfrastructure(IOptions<MQConsumerOptions> options, ILogger logger, IServiceScopeFactory ServiceScopeFactory)
        {
            _ServiceScopeFactory = ServiceScopeFactory;
            _logger=logger;
            try
            {
                _options = options.Value;
                var factory = new RabbitMQ.Client.ConnectionFactory()
                {
                    UserName = _options.username,
                    Password = _options.password,
                    HostName = _options.uri,
                    Port = _options.port,
                    VirtualHost = _options.virtualhost,
                    RequestedHeartbeat = TimeSpan.FromMinutes(_options.heartbeat),
                    // AutomaticRecoveryEnabled = true
                };
                var connection = factory.CreateConnection();
                channel = connection.CreateModel();
                channel.BasicQos(0, 10, false);
            }
            catch (Exception ex)
            {
                _logger.LogError("couldn't create connection using specified paramaeters : @params", options,ex);
            }
        }
        public void ConfigureConsumer(Func<byte[], IServiceScopeFactory, bool> consumeAction)
        {
            try { 
            var Consumer = new EventingBasicConsumer(channel);

            Consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var ConsumeResult = consumeAction(body,_ServiceScopeFactory);

                if (ConsumeResult)
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    _logger.LogError("Failed consuming execution message: @body requeing. ", ea.Body);
                    channel.BasicNack(ea.DeliveryTag, false, requeue: true);
                }
            };

            channel.BasicConsume(_options.queuename,
                    autoAck: false,
                    consumer: Consumer);
            }
            catch (Exception ex)
            {
                _logger.LogError("couldn't connect the consumer to the Queue: @params", ex);
            }
        }
    }
}
