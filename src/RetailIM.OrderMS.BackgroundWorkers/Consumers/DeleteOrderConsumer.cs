using RetailIM.Model.Dto;
using RetailIM.Services.Orders;
using Shared.Common.Serialization;
using Shared.MessageBus;
using Shared.MessageBus.Consumer;
using Shared.ROP;
using System;
using System.Threading.Tasks;


namespace RetailIM.OrderMS.BackgroundWorkers.Consumers
{
    public class DeleteOrderConsumer : ConsumerBase<Guid, DeleteOrderResponseDto>
    {
        private readonly DeleteOrder _deleteOrder;

        public DeleteOrderConsumer(RabbitMQSettings settings, ISerializer serializer, DeleteOrder deleteOrder)
            : base(settings, serializer, QueueName.CancelOrders)
        {
            _deleteOrder = deleteOrder;
        }

        protected override async Task<Result<DeleteOrderResponseDto>> CallService(Guid objDto)
        {
            return await _deleteOrder.Execute(objDto);
        }
    }
}
