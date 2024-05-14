using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordApi.Models;

/// <summary>
/// This class represents a user in the database.
/// </summary>
public class UserModel {
    /// <summary>
    /// This is the primary key for the user.
    /// </summary>
    [Key]
    [Required]
    [MinLength(3), MaxLength(255)]
    public string Name { get; set; }

    // Backing field for password
    [NotMapped]
    [Required]
    private string _password;

    /// <summary>
    /// This is the password for the user.
    /// </summary>
    [Required]
    [MinLength(8), MaxLength(255)]
    public string Password {
        get => _password;
        set => _password = BCrypt.Net.BCrypt.HashPassword(value);
    }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}