namespace RetailIM.WebApi.Dispatcher.Orders
{
    public class OrderQueues
    {
        public readonly OrderCreationProducerQueue OrderCreationProducerQueue;
        public readonly DeleteOrderProducerQueue DeleteOrderProducerQueue;
        public readonly PaginatedOrderProducerQueue PaginatedOrdersProducerQueue;
        public readonly GetOrderProducerQueue GetOrderProducerQueue;
        public readonly UpdateOrderDeliveryProducerQueue UpdateOrderDeliveryQueue;
        public readonly UpdateOrderProductsProducerQueue UpdateOrderProductsProducerQueue;

        public OrderQueues(OrderCreationProducerQueue orderCreationProducerQueue, DeleteOrderProducerQueue deleteOrderProducerQueue,
            PaginatedOrderProducerQueue paginatedOrderProducerQueue, GetOrderProducerQueue getOrderProducerQueue, UpdateOrderDeliveryProducerQueue updateOrderDeliveryQueue,
            UpdateOrderProductsProducerQueue updateOrderProductsProducerQueue)
        {
            OrderCreationProducerQueue = orderCreationProducerQueue;
            DeleteOrderProducerQueue = deleteOrderProducerQueue;
            PaginatedOrdersProducerQueue = paginatedOrderProducerQueue;
            GetOrderProducerQueue = getOrderProducerQueue;
            UpdateOrderDeliveryQueue = updateOrderDeliveryQueue;
            UpdateOrderProductsProducerQueue = updateOrderProductsProducerQueue;
        }
    }
}
