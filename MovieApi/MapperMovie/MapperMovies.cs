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
        CreateMap<Movie, UpdateMovieDto>().ReverseMap();

        CreateMap<AppUser, DataUserDto>().ReverseMap();
        CreateMap<AppUser, UserDto>().ReverseMap();
    }
}