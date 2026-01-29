using System.ComponentModel.DataAnnotations;

namespace MovieApi.Models.Dtos;

public class CategoryDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Category name is required")]
    [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    public string Name { get; set; }

    public DateTime CreatedDate { get; set; }
}