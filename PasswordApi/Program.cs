using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PasswordApi.Database;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "User API",
            Description = "User API endpoints",
            Version = "v1"
        });
    var filename = Assembly.GetExecutingAssembly().GetName().Name + ".xml";
    var filepath = Path.Combine(AppContext.BaseDirectory, filename);
    options.IncludeXmlComments(filepath);
});

// Add db context
builder.Services.AddDbContext<UserDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserDb"));
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();