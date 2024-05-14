using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PasswordApi.Models;

namespace PasswordApi.Database;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options) {
    public DbSet<UserModel> Users { get; init; }
}