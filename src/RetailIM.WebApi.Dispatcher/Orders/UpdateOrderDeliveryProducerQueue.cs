using RetailIM.Model;
using RetailIM.Model.Dto;
using Shared.Common.Serialization;
using Shared.MessageBus;
using Shared.MessageBus.Producer;

namespace RetailIM.WebApi.Dispatcher.Orders
{
    public class UpdateOrderDeliveryProducerQueue : MessageProducer<OrderDelivery, UpdateOrderDeliveryResponseDto>
    {
        public UpdateOrderDeliveryProducerQueue(RabbitMQSettings settings, ISerializer serializer)
            : base(settings, QueueName.UpdateOrderDelivery, serializer)
        {
        }
    }
}
