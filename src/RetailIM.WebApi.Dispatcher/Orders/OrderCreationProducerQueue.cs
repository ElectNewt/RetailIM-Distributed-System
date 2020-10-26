using RetailIM.Model.Dto;
using Shared.Common.Serialization;
using Shared.MessageBus;
using Shared.MessageBus.Producer;

namespace RetailIM.WebApi.Dispatcher.Orders
{
    public class OrderCreationProducerQueue : MessageProducer<OrderCreationDto, OrderCreationResponseDto>
    {
        public OrderCreationProducerQueue(RabbitMQSettings settings, ISerializer serializer)
            : base(settings, QueueName.CreateOrder, serializer)
        {
        }
    }
}
