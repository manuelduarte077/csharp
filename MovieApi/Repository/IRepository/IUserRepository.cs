using MovieApi.Models;
using MovieApi.Models.Dtos;

namespace MovieApi.Repository.IRepository;

public interface IUserRepository
{
    
    ICollection<User> GetAll();
    
    User GetById(int id);
    
    bool IsUniqueUsername(string username);
    
    Task<LoginUserResponseDto> Login(LoginUserDto loginUserDto);
    
    Task<User> Register(RegisterUserDto registerUserDto);
    
}