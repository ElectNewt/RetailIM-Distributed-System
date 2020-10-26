using RetailIM.Model;
using RetailIM.Model.Dto;
using RetailIM.Services.Orders;
using Shared.Common.Serialization;
using Shared.MessageBus;
using Shared.MessageBus.Consumer;
using Shared.ROP;
using System.Threading.Tasks;


namespace RetailIM.OrderMS.BackgroundWorkers.Consumers
{
    public class UpdateOrderDeliveryConsumer : ConsumerBase<OrderDelivery, UpdateOrderDeliveryResponseDto>
    {
        private readonly UpdateOrderDelivery _service;

        public UpdateOrderDeliveryConsumer(RabbitMQSettings settings, ISerializer serializer, UpdateOrderDelivery service)
            : base(settings, serializer, QueueName.UpdateOrderDelivery)
        {
            _service = service;
        }

        protected override async Task<Result<UpdateOrderDeliveryResponseDto>> CallService(OrderDelivery objDto)
        {
            return await _service.Execute(objDto);
        }
    }
}