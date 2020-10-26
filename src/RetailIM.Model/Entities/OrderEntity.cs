using System;

namespace RetailIM.Model.Entities
{
    public class OrderEntity
    {
        public readonly Guid OrderId;
        public readonly DateTime CreationTimeUtc;

        public OrderEntity(Guid orderid, DateTime creationtimeutc)
        {
            OrderId = orderid;
            CreationTimeUtc = creationtimeutc;
        }

        //Dapper constructor
        public OrderEntity(string orderid, DateTime creationtimeutc)
        {
            OrderId = new Guid(orderid);
            CreationTimeUtc = creationtimeutc;
        }
    }
}
