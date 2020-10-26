using RetailIM.Model.Dto;
using Shared.Common.Serialization;
using Shared.MessageBus;
using Shared.MessageBus.Producer;
using System;

namespace RetailIM.WebApi.Dispatcher.Orders
{
    public class GetOrderProducerQueue : MessageProducer<Guid, GetOrderResponseDto>
    {
        public GetOrderProducerQueue(RabbitMQSettings settings, ISerializer serializer)
            : base(settings, QueueName.RetrieveOrder, serializer)
        {
        }
    }
}
