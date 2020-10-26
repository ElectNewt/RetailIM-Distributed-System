using RetailIM.Model.Dto;
using RetailIM.Services.Orders;
using Shared.Common.Serialization;
using Shared.MessageBus;
using Shared.MessageBus.Consumer;
using Shared.ROP;
using System.Threading.Tasks;

namespace RetailIM.OrderMS.BackgroundWorkers.Consumers
{
    public class OrderCreationConsumer : ConsumerBase<OrderCreationDto, OrderCreationResponseDto>
    {
        private readonly CreateOrder _createOrder;

        public OrderCreationConsumer(RabbitMQSettings settings, ISerializer serializer, CreateOrder createOrder)
            : base(settings, serializer, QueueName.CreateOrder)
        {
            _createOrder = createOrder;
        }

        protected override async Task<Result<OrderCreationResponseDto>> CallService(OrderCreationDto objDto)
        {
            return await _createOrder.Execute(objDto);
        }
    }
}
