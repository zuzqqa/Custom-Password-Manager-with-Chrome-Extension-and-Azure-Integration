using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PasswordApi.Database;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(corsOptions => {
    corsOptions.AddDefaultPolicy(policyBuilder => {
        policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

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
app.UseSwaggerUI(options => {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
    options.DocumentTitle = "User API";
});


app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors();

app.MapControllers();

app.Run();