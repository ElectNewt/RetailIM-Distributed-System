using RetailIM.Model.Dto;
using System;
using System.Collections.Generic;

namespace RetailIM.Model
{
    public class OrderItems
    {
        public Guid OrderId { get; set; }
        public List<ProductQuantityDto> ProductsQty { get; set; }

        public OrderItems() { }

        public OrderItems(Guid orderId, List<ProductQuantityDto> productsQty)
        {
            OrderId = orderId;
            ProductsQty = productsQty;
        }
    }
}
