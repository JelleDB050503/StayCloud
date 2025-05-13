using System.ComponentModel.DataAnnotations;

public class PromoteUserRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;
}