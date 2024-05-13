using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PasswordDbFunction.Models;
using PasswordDbFunction.Models;

namespace PasswordDbFunction.DbService;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options) {
    public DbSet<UserModel> Users { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        //if (optionsBuilder.IsConfigured) return;

        var connectionString = new SqlConnectionStringBuilder {
            DataSource = "passworddbsqlserver.database.windows.net",
            InitialCatalog = "passworddb",
            UserID = "passwordDbAdmin",
            Password = "123Abc321!",
            TrustServerCertificate = true,
        };
        optionsBuilder.UseSqlServer(connectionString.ConnectionString);
    }
}