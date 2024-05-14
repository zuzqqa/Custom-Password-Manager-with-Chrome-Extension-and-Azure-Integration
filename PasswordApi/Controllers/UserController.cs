using Microsoft.AspNetCore.Mvc;
using PasswordApi.Database;
using PasswordApi.Models;

namespace PasswordApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(ILogger<UserController> logger, UserDbContext userDb) : Controller {
    // GET: /<controller>/
    [HttpGet("all")]
    public IEnumerable<UserModel> Get() => [.. userDb.Users];

    // GET: /<controller>/name
    [HttpPost("create")]
    public async Task<IActionResult> Post([FromBody] UserModel user) {
        try {
            await userDb.Users.AddAsync(user);
            await userDb.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e) {
            logger.LogError(e.Message, user);
            return BadRequest(e.Message);
        }
    }

    // POST: /<controller>/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserToValidate user) {
        var userInDb = await userDb.Users.FindAsync(user.Name);
        if (userInDb is null) return NotFound();
        if (BCrypt.Net.BCrypt.Verify(user.Password, userInDb.Password)) return Ok();
        return Unauthorized();
    }

    // PUT: /<controller>/update
    [HttpPut("update")]
    public async Task<IActionResult> Put([FromBody] UserModel user) {
        var userInDb = await userDb.Users.FindAsync(user.Name);
        if (userInDb is null) return NotFound();
        userInDb.Password = user.Password;
        userInDb.UpdatedAt = DateTime.Now;
        await userDb.SaveChangesAsync();
        return Ok();
    }

    // DELETE: /<controller>/delete
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] UserToValidate user) {
        var userInDb = await userDb.Users.FindAsync(user.Name);
        if (userInDb is null) return NotFound();
        userDb.Users.Remove(userInDb);
        await userDb.SaveChangesAsync();
        return Ok();
    }
}