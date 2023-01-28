using HotelListing.API.Contracts;
using HotelListing.API.Data;

namespace HotelListing.API.Respository;

public class HotelsRepository: GenericRepository<Hotel>, IHotelsRepository
{
    public HotelsRepository(HotelListingDbContext context) : base(context)
    {
    }
}