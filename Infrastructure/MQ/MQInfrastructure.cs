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
    public class MQInfrastructure : IMQInfrastructure
    {
        private readonly MQConsumerOptions _ConsumerOptions;
        private readonly MQPublisherOptions _PubOptions;
        private readonly IModel channel;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _ServiceScopeFactory;
        public MQInfrastructure(IOptions<MQConsumerOptions> ConsumerOptions, IOptions<MQPublisherOptions> PubOptions, ILogger logger, IServiceScopeFactory ServiceScopeFactory)
        {
            _ServiceScopeFactory = ServiceScopeFactory;
            _logger = logger;
            try
            {
                _ConsumerOptions = ConsumerOptions.Value;
                _PubOptions = PubOptions.Value;
                var factory = new RabbitMQ.Client.ConnectionFactory()
                {
                    UserName = _ConsumerOptions.username,
                    Password = _ConsumerOptions.password,
                    HostName = _ConsumerOptions.uri,
                    Port = _ConsumerOptions.port,
                    VirtualHost = _ConsumerOptions.virtualhost,
                    RequestedHeartbeat = TimeSpan.FromMinutes(_ConsumerOptions.heartbeat),
                    // AutomaticRecoveryEnabled = true
                };
                var connection = factory.CreateConnection();
                channel = connection.CreateModel();
                channel.BasicQos(0, 10, false);
                channel.ExchangeDeclare(exchange: _PubOptions.exchange, type: ExchangeType.Direct);
            }
            catch (Exception ex)
            {
                _logger.LogError("couldn't create connection using specified paramaeters : @params", _ConsumerOptions, ex);
            }
        }
        public void PublishMessage(string message)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: _PubOptions.exchange,
                                     routingKey: _PubOptions.routingkey,
                                     basicProperties: null,
                                     body: body);
                _logger.LogInformation("Sent message @message to exchange @ex", message, _PubOptions.exchange);
            }
            catch (Exception ex)
            {
                _logger.LogError("failed to Send message @message", message,ex);

            }
        }
        public void ConfigureConsumer(Func<byte[], IServiceScopeFactory, bool> consumeAction)
        {
            try
            {
                var Consumer = new EventingBasicConsumer(channel);

                Consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var ConsumeResult = consumeAction(body, _ServiceScopeFactory);

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

                channel.BasicConsume(_ConsumerOptions.queuename,
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
