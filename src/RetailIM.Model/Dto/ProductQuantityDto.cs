using System;

namespace RetailIM.Model.Dto
{
    public class ProductQuantityDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public ProductQuantityDto() { }
    }
}
