using RetailIM.Model.Dto;
using Shared.Common.Serialization;
using Shared.MessageBus;
using Shared.MessageBus.Producer;

namespace RetailIM.WebApi.Dispatcher.Orders
{
    public class PaginatedOrderProducerQueue : MessageProducer<int, PaginatedOrdersResponseDto>
    {
        public PaginatedOrderProducerQueue(RabbitMQSettings settings, ISerializer serializer)
            : base(settings, QueueName.RetrievePaginatedOrder, serializer)
        {
        }
    }
}

