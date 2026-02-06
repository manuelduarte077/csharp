using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MovieApi.Utils;

namespace MovieApi.Models;

public class Movie
{
    [Key] public int Id { get; set; }
    [Required] public string Title { get; set; }
    public string Description { get; set; }
    public int Duration { get; set; }
    public string Language { get; set; }
    public string? ImageUrl { get; set; }
    public string? LocationImageUrl { get; set; }
    public string ReleaseDate { get; set; }
    public ClassificationType Classification { get; set; }
    public DateTime CreatedDate { get; set; }

    // Category Relations
    public int CategoryId { get; set; }
    [ForeignKey("CategoryId")] public Category Category { get; set; }
}