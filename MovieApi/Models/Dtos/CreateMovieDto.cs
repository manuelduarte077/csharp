using MovieApi.Utils;

namespace MovieApi.Models.Dtos;

public class CreateMovieDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int Duration { get; set; }
    public string Language { get; set; }
    public string ImageUrl { get; set; }
    public DateTime ReleaseDate { get; set; }
    public ClassificationType Classification { get; set; }
    public int CategoryId { get; set; }
}