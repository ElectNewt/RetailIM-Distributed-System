namespace RetailIM.Model.Dto
{
    public class DeliveryDto
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }

        public DeliveryDto(string country, string city, string street)
        {
            Country = country;
            City = city;
            Street = street;
        }

        public DeliveryDto() { }
    }
}
