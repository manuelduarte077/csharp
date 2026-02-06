using MovieApi.Utils;

namespace MovieApi.Models.Dtos;

public class UpdateMovieDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Duration { get; set; }
    public string Language { get; set; }
    public string? ImageUrl { get; set; }
    public string? LocationImageUrl { get; set; }
    public IFormFile? Image { get; set; }
    public DateTime ReleaseDate { get; set; }
    public ClassificationType Classification { get; set; }
    public int CategoryId { get; set; }
}