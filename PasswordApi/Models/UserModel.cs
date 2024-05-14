using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordApi.Models;

public class UserModel {
    [Key]
    [Required]
    [MinLength(8), MaxLength(255)]
    public string Name { get; set; }

    // Backing field for password
    [NotMapped]
    [Required]
    private string _password;

    [Required]
    [MinLength(8), MaxLength(255)]
    public string Password {
        get => _password;
        set => _password = BCrypt.Net.BCrypt.HashPassword(value);
    }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}