using Authentication_Authoriztion.Data.Models;
using Authentication_Authoriztion.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication_Authoriztion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<UserModel> _userManager;

        public AccountController(IConfiguration configuration, UserManager<UserModel> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }
        [HttpPost("Dummy-Login")]
        public Results<Ok<TokenDto>, UnauthorizedHttpResult>
            DummyLogin(LoginDto credentials)
        {
            if (credentials.userEmail != "basem@gmail.com")
                return TypedResults.Unauthorized();
            if (credentials.password.ToString().Length < 4)
                return TypedResults.Unauthorized();
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email,credentials.userEmail),
                new Claim(ClaimTypes.NameIdentifier,$"{Guid.NewGuid()}")
            };
            var tokenDto = GenerateToken(claims);
            return TypedResults.Ok(tokenDto);
        }

        [HttpPost("Login")]
        public async Task<Results<Ok<TokenDto>, UnauthorizedHttpResult>>
            Login(LoginDto credentials)
        {
            var user = await _userManager.FindByEmailAsync(credentials.userEmail);
            if (user is null)
            {
                return TypedResults.Unauthorized();
            }
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, credentials.password);
            if (!isPasswordValid)
                return TypedResults.Unauthorized();
            var claims = await _userManager.GetClaimsAsync(user);
            var tokenDto = GenerateToken(claims.ToList());
            return TypedResults.Ok(tokenDto);
        }
        private TokenDto GenerateToken(List<Claim> claims)
        {
            //secret key change to symmetric key req for token
            var secretKey = _configuration.GetValue<string>("SecretKey")!;
            var secretKeyInBytes = Encoding.UTF8.GetBytes(secretKey);
            var key = new SymmetricSecurityKey(secretKeyInBytes);
            //token generation
            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddHours(1),
                claims: claims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
            // convert to string to return it 
            Console.WriteLine(token);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return new TokenDto(tokenString, token.ValidTo);
        }
        [HttpPost("Register/user")]
        public async Task<Results<NoContent, BadRequest<List<string>>>>
            Register(RegisterDto registerDto)
        {
            var user = new UserModel()
            {
                UserName = registerDto.userName,
                Email = registerDto.userEmail,
                Role = "User"
            };
            var creationResult = await _userManager.CreateAsync(user, registerDto.password);
            if (!creationResult.Succeeded)
            {
                var errors = creationResult.Errors.Select(e => e.Description).ToList();
                return TypedResults.BadRequest(errors);
            }
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role,user.Role)
            };
            await _userManager.AddClaimsAsync(user, claims);
            return TypedResults.NoContent();
        }
        [HttpPost("Register/admin")]
        public async Task<Results<NoContent, BadRequest<List<string>>>>
            RegisterAdmin(RegisterDto registerDto)
        {
            var user = new UserModel()
            {
                UserName = registerDto.userName,
                Email = registerDto.userEmail,
                Role = "Admin"
            };
            var creationResult = await _userManager.CreateAsync(user, registerDto.password);
            if (!creationResult.Succeeded)
            {
                var errors = creationResult.Errors.Select(e => e.Description).ToList();
                return TypedResults.BadRequest(errors);
            }
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role,user.Role)
            };
            await _userManager.AddClaimsAsync(user, claims);
            return TypedResults.NoContent();
        }
    }
}
