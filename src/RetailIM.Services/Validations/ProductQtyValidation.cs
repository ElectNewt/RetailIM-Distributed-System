using RetailIM.Model.Dto;
using RetailIM.Model.Entities;
using Shared.ROP;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RetailIM.Services.Validations
{
    public static class ProductQtyValidation
    {
        public static Result<ProductQuantityDto> ValidateInsertion(this ProductQuantityDto productQty, ReadOnlyCollection<ProductEntity> allProducts)
        {
            ProductEntity lookupProduct = allProducts.FirstOrDefault(a => a.ProductId == productQty.ProductId);

            if (lookupProduct == null)
                return Result.Failure<ProductQuantityDto>($"The item you are looking for {productQty.ProductId} does not exist");

            if (lookupProduct.TotalStock <= (lookupProduct.AllocatedStock + productQty.Quantity))
                return Result.Failure<ProductQuantityDto>($"There is not enough stock for the item {productQty.ProductId}");

            return productQty;
        }

        public static Result<ProductQuantityDto> ValidateUpdate(this ProductQuantityDto productQty, ReadOnlyCollection<OrderProductEntity> currentOrderQty, ReadOnlyCollection<ProductEntity> allProducts)
        {
            ProductEntity lookupProduct = allProducts.FirstOrDefault(a => a.ProductId == productQty.ProductId);
            OrderProductEntity? currentOrderProduct = currentOrderQty.FirstOrDefault(a => a.ProductId == productQty.ProductId);

            if (lookupProduct == null)
                return Result.Failure<ProductQuantityDto>($"The item you are looking for {productQty.ProductId} does not exist");


            if (currentOrderProduct != null)
            {
                //Comparing current stock  against the allocated - the old older stock + new stock
                if (lookupProduct.TotalStock <= (lookupProduct.AllocatedStock - currentOrderProduct.Quantity + productQty.Quantity))
                    return Result.Failure<ProductQuantityDto>($"There is not enough stock for the item {productQty.ProductId}");
            }
            else
            {
                if (lookupProduct.TotalStock < (lookupProduct.AllocatedStock + productQty.Quantity))
                    return Result.Failure<ProductQuantityDto>($"There is not enough stock for the item {productQty.ProductId}");
            }

            return productQty;
        }

        public static List<ProductQuantityDto> EnsureUniqueItems(this List<ProductQuantityDto> products)
        {
            return products.GroupBy(a => a.ProductId)
                .Select(a => new ProductQuantityDto()
                {
                    ProductId = a.Key,
                    Quantity = a.Sum(a => a.Quantity)
                }).ToList();
        }
    }
}
