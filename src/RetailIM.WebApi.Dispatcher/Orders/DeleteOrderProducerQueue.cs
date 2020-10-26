using RetailIM.Model.Dto;
using Shared.Common.Serialization;
using Shared.MessageBus;
using Shared.MessageBus.Producer;
using System;

namespace RetailIM.WebApi.Dispatcher.Orders
{
    public class DeleteOrderProducerQueue : MessageProducer<Guid, DeleteOrderResponseDto>
    {
        public DeleteOrderProducerQueue(RabbitMQSettings settings, ISerializer serializer)
            : base(settings, QueueName.CancelOrders, serializer)
        {
        }
    }
}
