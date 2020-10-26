using RetailIM.Model.Dto;
using RetailIM.Services.Orders;
using Shared.Common.Serialization;
using Shared.MessageBus;
using Shared.MessageBus.Consumer;
using Shared.ROP;
using System.Threading.Tasks;


namespace RetailIM.OrderMS.BackgroundWorkers.Consumers
{
    public class PaginatedOrdersConsumer : ConsumerBase<int, PaginatedOrdersResponseDto>
    {
        private readonly RetrievePaginatedOrders _service;

        public PaginatedOrdersConsumer(RabbitMQSettings settings, ISerializer serializer, RetrievePaginatedOrders service)
            : base(settings, serializer, QueueName.RetrievePaginatedOrder)
        {
            _service = service;
        }

        protected override async Task<Result<PaginatedOrdersResponseDto>> CallService(int objDto)
        {
            return await _service.Execute(objDto);
        }
    }
}
