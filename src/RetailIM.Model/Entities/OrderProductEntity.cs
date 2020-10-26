using System;

namespace RetailIM.Model.Entities
{
    public class OrderProductEntity
    {
        public readonly Guid OrderId;
        public readonly Guid ProductId;
        public readonly int Quantity;

        public OrderProductEntity(Guid orderid, Guid productid, int quantity)
        {
            OrderId = orderid;
            ProductId = productid;
            Quantity = quantity;
        }

        //dapper constructor
        public OrderProductEntity(string orderid, string productid, int quantity)
        {
            OrderId = new Guid(orderid);
            ProductId = new Guid(productid);
            Quantity = quantity;
        }
    }
}
