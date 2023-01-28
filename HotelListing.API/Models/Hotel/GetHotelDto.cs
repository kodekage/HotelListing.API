using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.Hotel;

public class GetHotelDto : BaseHotelDto
{
    public int Id { get; set; }
}



