namespace MovieApi.Models.Dtos;

public class LoginUserResponseDto
{
    public User User { get; set; }
    public string Role { get; set; }
    public string Token { get; set; }
}