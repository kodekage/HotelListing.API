using HotelListing.API.Models.ApiUser;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Contracts;

public interface IAuthManager
{
    Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto);
    
    Task<AuthResponseDto> Login(LoginDto loginDto);

    Task<string> CreateRefreshToken(string userEmail);

    Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request);
}