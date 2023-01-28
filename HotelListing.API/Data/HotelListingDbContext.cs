using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Data;

public class HotelListingDbContext: DbContext
{
    public HotelListingDbContext(DbContextOptions options): base(options)
    {
        
    }

    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Country> Countries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Country>().HasData(
            new Country()
            {
                Id = 1,
                Name = "Jamaica",
                ShortName = "JM"
            },
            new Country()
            {
                Id = 2,
                Name = "Bahamas",
                ShortName = "BS"
            },
            new Country()
            {
                Id = 3,
                Name = "Nigeria",
                ShortName = "NG"
            }
        );

        modelBuilder.Entity<Hotel>().HasData(
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