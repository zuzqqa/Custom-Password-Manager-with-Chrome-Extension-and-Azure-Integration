using Microsoft.AspNetCore.Mvc;
using PasswordDbApi.DbService;
using PasswordDbApi.Models;

namespace PasswordDbApi.Controllers;

[ApiController]
[Route("[controller]s")]
public class UserController(UserDbContext userDb) : Controller {
    // List all users
    [HttpGet("all")]
    public IActionResult Get() => Ok(userDb.Users.ToList());

    [HttpPost("save")]
    public async Task<IActionResult> Post(UserModel user) {
        try {
            userDb.Users.Add(user);
            await userDb.SaveChangesAsync();
            return Ok("User saved");
        }
        catch (Exception) {
            return new StatusCodeResult(500);
        }
    }

    [HttpPost("search")]
    public IActionResult Search(string name) {
        var user = userDb.Users.Find(name);
        return user == null ? NotFound("User not found") : Ok(user);
    }
}