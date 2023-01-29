namespace HotelListing.API.Models.ApiUser;

public class AuthResponseDto
{
    public string UserId { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}