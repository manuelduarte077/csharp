using AutoMapper;
using MovieApi.Models;
using MovieApi.Models.Dtos;

namespace MovieApi.MapperMovie;

public class MapperMovies : Profile
{
    public MapperMovies()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Category, CreateCategoryDto>().ReverseMap();

        CreateMap<Movie, MovieDto>().ReverseMap();
        CreateMap<Movie, CreateMovieDto>().ReverseMap();

        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, RegisterUserDto>().ReverseMap();
        CreateMap<User, LoginUserDto>().ReverseMap();
        CreateMap<User, DataUserDto>().ReverseMap();
        CreateMap<User, LoginUserResponseDto>().ReverseMap();
    }
}