namespace Shared.MessageBus
{
    public class QueueName
    {
        public const string CreateOrder = "create-order-queue";
        public const string UpdateOrderDelivery = "update-order-delivery-queue";
        public const string UpdateOrderProducts = "update-order-products-queue";
        public const string CancelOrders = "cancel-order-queue";
        public const string RetrieveOrder = "retrieve-order-queue";
        public const string RetrievePaginatedOrder = "retrieve-paginated-order-queue";
    }
}
