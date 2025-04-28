using EasyFly.Application.Dtos;

namespace EasyFly.Tests.Factories
{
    public static class PlaneFactory
    {
        public static PlaneDto Create()
        {
            var fake = new PlaneDto()
            {
                Name = Faker.Company.Name(),
                AvailableSeats = Faker.RandomNumber.Next(1, 33)
            };

            return fake;
        }
    }
}
