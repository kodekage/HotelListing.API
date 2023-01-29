using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using HotelListing.API.Contracts;
using HotelListing.API.Data;
using HotelListing.API.Models.ApiUser;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace HotelListing.API.Respository;

public class AuthManager : IAuthManager
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApiUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthManager(IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration)
    {
        _mapper = mapper;
        _userManager = userManager;
        _configuration = configuration;
    }
    public async Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto)
    {
        var user = _mapper.Map<ApiUser>(userDto);
        user.UserName = userDto.Email;

        var result = await _userManager.CreateAsync(user, userDto.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
        }

        return result.Errors;
    }

    public async Task<AuthResponseDto> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
            var validPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            
            if (user is null || !validPassword)
            {
                return default;
            }

            var token = await GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id
            };
    }

    public async Task<string> CreateRefreshToken(string userEmail)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        await _userManager.RemoveAuthenticationTokenAsync(user, "HotelListingApi", "RefreshToken");

        var newRreshToken = await _userManager.GenerateUserTokenAsync(user, "HotelListingApi", "RefreshToken");
        var result =
            await _userManager.SetAuthenticationTokenAsync(user, "HotelListingApi", "RefreshToken", newRreshToken);

        return newRreshToken;
    }

    public async Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
        var userName = tokenContent.Claims.ToList().FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Email)?.Value;
        var user = await _userManager.FindByNameAsync(userName);

        if (user == null)
        {
            return null;
        }

        var isValidRefreshToken =
            await _userManager.VerifyUserTokenAsync(user, "HotelListingApi", "RefreshToken", request.RefreshToken);

        if (!isValidRefreshToken)
        {
            return null;
        }
        
        var token = await GenerateToken(user);
        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            RefreshToken = await CreateRefreshToken(user.Email)
        };
        
    }

    private async Task<string> GenerateToken(ApiUser user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
        var userClaims = await _userManager.GetClaimsAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("uid", user.Id),
        }
            .Union(userClaims)
            .Union(roleClaims);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}