using System;

namespace RetailIM.Model.Entities
{
    public class DeliveryEntity
    {
        public readonly Guid? DeliveryId;
        public readonly string Country;
        public readonly string City;
        public readonly string Street;

        public DeliveryEntity(Guid? deliveryid, string country, string city, string street)
        {
            DeliveryId = deliveryid;
            Country = country;
            City = city;
            Street = street;
        }

        //dapper constructor
        public DeliveryEntity(string deliveryid, string country, string city, string street)
        {
            DeliveryId = new Guid(deliveryid);
            Country = country;
            City = city;
            Street = street;
        }
    }
}
