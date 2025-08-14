namespace Taskify.DTOs;

public record UserLoginDto
{
    [JsonProperty("username")]
    public string Username { get; set; }
    [JsonProperty("password")]
    public string Password { get; set; }
}
