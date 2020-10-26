using System.Collections.Generic;

namespace RetailIM.Model.Dto
{
    public class OrderCreationDto
    {
        public List<ProductQuantityDto> Products { get; set; }
        public DeliveryDto Delivery { get; set; }

        public OrderCreationDto() { }
    }
}
