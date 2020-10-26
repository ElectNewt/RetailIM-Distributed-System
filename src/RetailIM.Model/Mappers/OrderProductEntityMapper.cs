using RetailIM.Model.Dto;
using RetailIM.Model.Entities;

namespace RetailIM.Model.Mappers
{
    public static class OrderProductEntityMapper
    {
        public static ProductQuantityDto ToProductQuantityDto(this OrderProductEntity orderProduct)
        {
            return new ProductQuantityDto() { ProductId = orderProduct.ProductId, Quantity = orderProduct.Quantity };
        }
    }
}
