using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MovieApi.Data;
using MovieApi.Models;
using MovieApi.Models.Dtos;
using MovieApi.Repository.IRepository;

namespace MovieApi.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string keyScret;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    public UserRepository(ApplicationDbContext dbContext, IConfiguration config, UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager, IMapper mapper)
    {
        _dbContext = dbContext;
        keyScret = config.GetValue<string>("AppSettings:KeyScret");
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public ICollection<AppUser> GetAll()
    {
        return _dbContext.AppUsers.OrderBy(c => c.UserName).ToList();
    }

    public AppUser GetById(string id)
    {
        return _dbContext.AppUsers.FirstOrDefault(u => u.Id == id);
    }

    public bool IsUniqueUsername(string username)
    {
        var user = _dbContext.AppUsers.FirstOrDefault(u => u.UserName == username);
        if (user == null)
        {
            return true;
        }

        return false;
    }

    public async Task<LoginUserResponseDto> Login(LoginUserDto loginUserDto)
    {
        var user = _dbContext.AppUsers.FirstOrDefault(u =>
            u.UserName.ToLower() == loginUserDto.Username.ToLower());

        bool isValid = await _userManager.CheckPasswordAsync(user, loginUserDto.Password);


        if (user == null || !isValid)
        {
            return new LoginUserResponseDto()
            {
                Token = "",
                User = null,
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var jwtToken = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(keyScret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault())
            ]),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = jwtToken.CreateToken(tokenDescriptor);
        LoginUserResponseDto response = new LoginUserResponseDto()
        {
            Token = jwtToken.WriteToken(token),
            User = _mapper.Map<DataUserDto>(user),
        };

        return response;
    }

    public async Task<DataUserDto> Register(RegisterUserDto registerUserDto)
    {
        AppUser user = new AppUser()
        {
            UserName = registerUserDto.Username,
            Email = registerUserDto.Username,
            NormalizedEmail = registerUserDto.Username.ToUpper(),
            Name = registerUserDto.Name,
        };

        var result = await _userManager.CreateAsync(user, registerUserDto.Password);
        if (result.Succeeded)
        {
            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            await _userManager.AddToRoleAsync(user, "Admin");

            var userResult = _dbContext.AppUsers.FirstOrDefault(u => u.UserName == registerUserDto.Username);
            return _mapper.Map<DataUserDto>(userResult);
        }

        return new DataUserDto();
    }
}