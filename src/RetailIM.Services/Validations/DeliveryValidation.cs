using RetailIM.Model.Dto;
using Shared.ROP;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace RetailIM.Services.Validations
{
    public static class DeliveryValidation
    {
        public static Result<DeliveryDto> Validate(this DeliveryDto delivery)
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(delivery.City))
                errors.Add("City in the delivery does not exist");

            if (string.IsNullOrWhiteSpace(delivery.Country))
                errors.Add("Country in the delivery does not exist");

            if (string.IsNullOrWhiteSpace(delivery.Street))
                errors.Add("Street in the delivery does not exist");

            return errors.Any() ?
               Result.Failure<DeliveryDto>(errors.ToImmutableArray())
               : delivery;
        }

    }
}
