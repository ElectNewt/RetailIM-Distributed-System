using RetailIM.Model.Dto;
using RetailIM.Services.Validations;
using Shared.ROP;
using Xunit;

namespace RetailIM.UnitTest.Services.Validations
{
    public class Test_DeliveryValidation
    {

        [Fact]
        public void TestDelivery_AllCorrect()
        {
            DeliveryDto delivery = new DeliveryDto()
            {
                City = "city",
                Country = "country",
                Street = "street"
            };

            Result<DeliveryDto> result = delivery.Validate();

            Assert.True(result.Success);
            DeliveryDto updatedDelivery = result.Value;
            Assert.Equal(updatedDelivery, delivery);
        }

        [Theory]
        [InlineData("city", "country", "")]
        [InlineData("city", "", "street")]
        [InlineData("", "country", "street")]
        public void TestDelivery_Validate_inputs(string city, string country, string street)
        {
            DeliveryDto delivery = new DeliveryDto()
            {
                City = city,
                Country = country,
                Street = street
            };

            Result<DeliveryDto> result = delivery.Validate();

            Assert.False(result.Success);
            Assert.Single(result.Errors);
        }
    }
}
