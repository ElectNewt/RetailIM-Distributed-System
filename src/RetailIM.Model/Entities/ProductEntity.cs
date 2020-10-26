using System;

namespace RetailIM.Model.Entities
{
    public class ProductEntity
    {
        public readonly Guid ProductId;
        public readonly string Name;
        public readonly int TotalStock;
        public readonly int AllocatedStock;

        public ProductEntity(Guid productid, string name, int totalstock, int allocatedstock)
        {
            ProductId = productid;
            Name = name;
            TotalStock = totalstock;
            AllocatedStock = allocatedstock;
        }

        //Dapper constructor
        public ProductEntity(string productid, string name, int totalstock, int allocatedstock)
        {
            ProductId = new Guid(productid);
            Name = name;
            TotalStock = totalstock;
            AllocatedStock = allocatedstock;
        }
    }
}
