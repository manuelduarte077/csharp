using System.ComponentModel.DataAnnotations;

namespace MovieApi.Models;

public class Category
{
    [Key] public int Id { get; set; }
    [Required] public string Name { get; set; }

    [Required]
    [Display(Name = "Date Created")]
    public DateTime CreatedDate { get; set; }
}