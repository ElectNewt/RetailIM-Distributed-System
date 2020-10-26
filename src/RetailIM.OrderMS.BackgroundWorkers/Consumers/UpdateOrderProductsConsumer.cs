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
    public class UpdateOrderProductsConsumer : ConsumerBase<OrderItems, UpdateProductsResponseDto>
    {
        private readonly UpdateOrderItems _service;

        public UpdateOrderProductsConsumer(RabbitMQSettings settings, ISerializer serializer, UpdateOrderItems service)
            : base(settings, serializer, QueueName.UpdateOrderProducts)
        {
            _service = service;
        }

        protected override async Task<Result<UpdateProductsResponseDto>> CallService(OrderItems objDto)
        {
            return await _service.Execute(objDto);
        }
    }
}