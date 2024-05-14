using PasswordApi.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSwaggerGen(setup => {
    setup.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo {
        Title = "User API",
        Version = "v1"
    });
});

// Add db context
builder.Services.AddDbContext<UserDbContext>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();