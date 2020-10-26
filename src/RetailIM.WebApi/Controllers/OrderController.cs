using Microsoft.AspNetCore.Mvc;
using RetailIM.Model.Dto;
using RetailIM.WebApi.Dispatcher.Orders;
using Shared.Common.Serialization;
using Shared.MessageBus;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailIM.WebApi.Controllers
{

    [ApiController]
    [Route("[Controller]")]
    public class OrderController : Controller
    {
        //TODO: figure it out the problem with the conection being closed when producerQueue is injected
        private readonly RabbitMQSettings _settings;
        private readonly ISerializer _serializer;
        public OrderController(RabbitMQSettings settings, ISerializer serializer)
        {
            _serializer = serializer;
            _settings = settings;
        }

        [HttpPost("create")]
        public async Task<OperationResultDto<OrderCreationResponseDto>> Create(OrderCreationDto order)
        {
            using OrderCreationProducerQueue queue = new OrderCreationProducerQueue(_settings, _serializer);
            var result = await queue.CallAsync(order);
            return result;
        }

        [HttpPut("updatedelivery/{orderId}")]
        public async Task<OperationResultDto<UpdateOrderDeliveryResponseDto>> UpdateDelivery(Guid orderId, DeliveryDto delivery)
        {

            using UpdateOrderDeliveryProducerQueue queue = new UpdateOrderDeliveryProducerQueue(_settings, _serializer);
            var result = await queue.CallAsync(new Model.OrderDelivery(orderId, delivery));
            return result;
        }

        [HttpPut("updateproducts/{orderId}")]
        public async Task<OperationResultDto<UpdateProductsResponseDto>> UpdateProducts(Guid orderId, List<ProductQuantityDto> products)
        {
            using UpdateOrderProductsProducerQueue queue = new UpdateOrderProductsProducerQueue(_settings, _serializer);
            var result = await queue.CallAsync(new Model.OrderItems(orderId, products));
            return result;
        }

        [HttpDelete("{orderId}")]
        public async Task<OperationResultDto<DeleteOrderResponseDto>> Delete(Guid orderId)
        {
            using DeleteOrderProducerQueue queue = new DeleteOrderProducerQueue(_settings, _serializer);
            var result = await queue.CallAsync(orderId);
            return result;
        }

        [HttpGet("{orderId}")]
        public async Task<OperationResultDto<GetOrderResponseDto>> Get(Guid orderId)
        {
            using GetOrderProducerQueue queue = new GetOrderProducerQueue(_settings, _serializer);
            var result = await queue.CallAsync(orderId);
            return result;
        }

        [HttpGet("page/{pageNumber}")]
        public async Task<OperationResultDto<PaginatedOrdersResponseDto>> GetPaginated(int pageNumber)
        {
            using PaginatedOrderProducerQueue queue = new PaginatedOrderProducerQueue(_settings, _serializer);
            var result = await queue.CallAsync(pageNumber);
            return result;
        }

    }
}
