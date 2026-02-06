using MovieApi.Models;
using MovieApi.Models.Dtos;

namespace MovieApi.Repository.IRepository;

public interface IUserRepository
{
    ICollection<AppUser> GetAll();

    AppUser GetById(string id);

    bool IsUniqueUsername(string username);

    Task<LoginUserResponseDto> Login(LoginUserDto loginUserDto);

    Task<DataUserDto> Register(RegisterUserDto registerUserDto);
}