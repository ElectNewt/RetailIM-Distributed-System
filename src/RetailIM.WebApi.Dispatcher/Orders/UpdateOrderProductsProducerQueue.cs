using RetailIM.Model;
using RetailIM.Model.Dto;
using Shared.Common.Serialization;
using Shared.MessageBus;
using Shared.MessageBus.Producer;

namespace RetailIM.WebApi.Dispatcher.Orders
{
    public class UpdateOrderProductsProducerQueue : MessageProducer<OrderItems, UpdateProductsResponseDto>
    {
        public UpdateOrderProductsProducerQueue(RabbitMQSettings settings, ISerializer serializer)
            : base(settings, QueueName.UpdateOrderProducts, serializer)
        {
        }
    }
}
