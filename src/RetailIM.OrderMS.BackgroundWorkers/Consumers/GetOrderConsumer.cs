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
    public class GetOrderConsumer : ConsumerBase<Guid, GetOrderResponseDto>
    {
        private readonly RetrieveSingleOrder _service;

        public GetOrderConsumer(RabbitMQSettings settings, ISerializer serializer, RetrieveSingleOrder service)
            : base(settings, serializer, QueueName.RetrieveOrder)
        {
            _service = service;
        }

        protected override async Task<Result<GetOrderResponseDto>> CallService(Guid objDto)
        {
            return await _service.Execute(objDto);
        }
    }
}