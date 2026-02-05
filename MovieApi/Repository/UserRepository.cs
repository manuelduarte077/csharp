using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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

    public UserRepository(ApplicationDbContext dbContext, IConfiguration config)
    {
        _dbContext = dbContext;
        keyScret = config.GetValue<string>("AppSettings:KeyScret");
    }

    public ICollection<User> GetAll()
    {
        return _dbContext.Users.OrderBy(c => c.Username).ToList();
    }

    public User GetById(int id)
    {
        return _dbContext.Users.FirstOrDefault(u => u.Id == id);
    }

    public bool IsUniqueUsername(string username)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            return true;
        }

        return false;
    }

    public async Task<LoginUserResponseDto> Login(LoginUserDto loginUserDto)
    {
        var encryptPassword = getMD5(loginUserDto.Password);
        var user = _dbContext.Users.FirstOrDefault(u => u.Username.ToLower() == loginUserDto.Username.ToLower()
                                                        && u.Password == encryptPassword
        );

        if (user == null)
        {
            return new LoginUserResponseDto()
            {
                Token = "",
                User = null,
            };
        }

        var jwtToken = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(keyScret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =  new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = jwtToken.CreateToken(tokenDescriptor);
        LoginUserResponseDto response = new LoginUserResponseDto()
        {
            Token = jwtToken.WriteToken(token),
            User = user
        };
        
        return response;

    }

    public async Task<User> Register(RegisterUserDto registerUserDto)
    {
        var encryptPassword = getMD5(registerUserDto.Password);

        User user = new User()
        {
            Username = registerUserDto.Username,
            Password = encryptPassword,
            Name = registerUserDto.Name,
            Email = registerUserDto.Email,
            Role = registerUserDto.Role,
        };

        _dbContext.Add(user);
        await _dbContext.SaveChangesAsync();
        user.Password = encryptPassword;

        return user;
    }

    [Obsolete("Obsolete")]
    public static string getMD5(string valor)
    {
        MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
        data = x.ComputeHash(data);
        string resp = "";
        for (int i = 0; i < data.Length; i++)
            resp += data[i].ToString("x2").ToLower();
        return resp;
    }
}