using System;

namespace RetailIM.Model.Dto
{
    public class ProductInformationDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }

        public ProductInformationDto() { }

        public ProductInformationDto(Guid productId, string name, int quantity)
        {
            ProductId = productId;
            Name = name;
            Quantity = quantity;
        }
    }
}
