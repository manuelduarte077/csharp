using Microsoft.AspNetCore.Identity;

namespace MovieApi.Models;

public class AppUser : IdentityUser
{
    public string Name { get; set; }
}