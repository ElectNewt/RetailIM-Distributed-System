using System;
using System.Collections.Generic;

namespace RetailIM.Model.Dto
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public DateTime CreationDateUtc { get; set; }
        public DeliveryDto DeliveryDto { get; set; }
        public List<ProductInformationDto> Products { get; set; }
        public OrderDto() { }

        public OrderDto(Guid orderId, DateTime creationDateUtc, DeliveryDto deliveryDto, List<ProductInformationDto> products)
        {
            OrderId = orderId;
            CreationDateUtc = creationDateUtc;
            DeliveryDto = deliveryDto;
            Products = products;
        }
    }
}
