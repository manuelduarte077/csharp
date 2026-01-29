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
    }
} 