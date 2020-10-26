using RetailIM.Model.Dto;
using RetailIM.Model.Entities;
using System;

namespace RetailIM.Model.Mappers
{
    public static class DeliveryMapper
    {

        public static DeliveryEntity ToEntity(this DeliveryDto delivery)
        {
            return new DeliveryEntity((Guid?)null, delivery.Country, delivery.City, delivery.Street);
        }

        public static DeliveryDto ToDto(this DeliveryEntity delivery)
        {
            return new DeliveryDto(delivery.Country, delivery.City, delivery.Street);
        }

    }
}
