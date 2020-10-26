using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RetailIM.Model.Dto;
using Shared.Common.Serialization;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.MessageBus.Producer
{
    public interface IMessageProducer<TSent, TResponseDto>
    {
        Task<TResponseDto> CallAsync(TSent obj, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class MessageProducer<TSent, TResponseDto> : IMessageProducer<TSent, OperationResultDto<TResponseDto>>, IDisposable
    where TResponseDto : class
    {
        private readonly ISerializer _serializer;
        private readonly string _queueName;
        private readonly string _responseQueueName;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<OperationResultDto<TResponseDto>>> _callbackMapper =
                new ConcurrentDictionary<string, TaskCompletionSource<OperationResultDto<TResponseDto>>>();
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;

        public MessageProducer(RabbitMQSettings settings, string queueName, ISerializer serializer)
        {
            _serializer = serializer;
            _queueName = queueName;

            var factory = new ConnectionFactory() { HostName = settings.Hostname, UserName = settings.Username, Password = settings.Password };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _responseQueueName = $"{queueName}-response";
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += (model, ea) =>
            {
                if (!_callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out TaskCompletionSource<OperationResultDto<TResponseDto>> tcs))
                {
                    return;
                }

                var res = _serializer.DeserializeObject<OperationResultDto<TResponseDto>>(ea.Body.ToArray());
                tcs.TrySetResult(res);
            };

        }

        public Task<OperationResultDto<TResponseDto>> CallAsync(TSent obj, CancellationToken cancellationToken = default(CancellationToken))
        {
            IBasicProperties props = _channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = _responseQueueName;
            var messageBytes = _serializer.SerializeObjectToByteArray(obj);
            var tcs = new TaskCompletionSource<OperationResultDto<TResponseDto>>();
            _callbackMapper.TryAdd(correlationId, tcs);

            _channel.BasicPublish(
                exchange: "",
                routingKey: _queueName,
                basicProperties: props,
                body: messageBytes);

            _channel.BasicConsume(
                consumer: _consumer,
                queue: _responseQueueName,
                autoAck: true);

            cancellationToken.Register(() => _callbackMapper.TryRemove(correlationId, out var tmp));
            return tcs.Task;
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
