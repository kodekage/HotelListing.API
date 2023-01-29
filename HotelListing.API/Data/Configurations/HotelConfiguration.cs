using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.API.Data.Configurations;

public class HotelConfiguration: IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.HasData(
            new Hotel()
            {
                Id = 1,
                Name = "Sandiago Berus",
                Address = "Shanty Town",
                CountryId = 1,
                Rating = 4
            },
            new Hotel()
            {
                Id = 2,
                Name = "Hotel De Lazaz",
                Address = "Shanty Town",
                CountryId = 2,
                Rating = 2
            },
            new Hotel()
            {
                Id = 3,
                Name = "Sandiago Berus III",
                Address = "Shanty Town",
                CountryId = 3,
                Rating = 5
            }
        );
    }
}