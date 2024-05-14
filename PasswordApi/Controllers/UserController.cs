using Microsoft.AspNetCore.Mvc;
using PasswordApi.Database;
using PasswordApi.Models;

namespace PasswordApi.Controllers;

/// <summary>
/// This class represents the user controller.
/// </summary>
/// <param name="logger">Instance implementing ILogger used for logging</param>
/// <param name="userDb">Db context for user database interactions</param>
[ApiController]
[Route("[controller]")]
public class UserController(ILogger<UserController> logger, UserDbContext userDb) : Controller {
    // GET: /<controller>/
    /// <summary>
    /// Returns all users in the database.
    /// </summary>
    /// <returns>List of users</returns>
    /// <response code="200">Returns all users</response>
    /// <response code="500">If there was an error</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("all")]
    public IEnumerable<UserModel> Get() => [.. userDb.Users];

    // Post: /<controller>/create
    /// <summary>
    /// Creates a new user in the database.
    /// </summary>
    /// <param name="user">Simplified user class which contains username and password only</param>
    /// <returns>Appropriate status code response</returns>
    /// <response code="201">If everything is correct and user was created</response>
    /// <response code="409">If username already exists</response>
    /// <response code="400">If exception is thrown</response>
    /// <response code="500">If there was an error</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    [HttpPost("create")]
    public async Task<IActionResult> Post([FromBody] UserModel user) {
        if (await userDb.Users.FindAsync(user.Name) is not null)
            return Conflict($"The username {user.Name} already exists.");

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
    /// <summary>
    /// This method is used to authenticate a user.
    /// </summary>
    /// <param name="user">Simplified user class which contains username and password only</param>
    /// <returns>Appropriate status code response</returns>
    /// <response code="200">If everything is correct</response>
    /// <response code="404">If user does not exist</response>
    /// <response code="401">If password is incorrect</response>
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)] // No content
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound, Type = typeof(void))] // No content
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized, Type = typeof(void))] // No content
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserToValidate user) {
        var userInDb = await userDb.Users.FindAsync(user.Name);
        if (userInDb is null) return NotFound();
        if (BCrypt.Net.BCrypt.Verify(user.Password, userInDb.Password)) return Ok();
        return Unauthorized();
    }

    // PUT: /<controller>/update
    /// <summary>
    /// This method is used to update a user's password.
    /// </summary>
    /// <param name="user">User instance to be updated</param>
    /// <returns>Appropriate status code response</returns>
    /// <response code="200">If everything is correct</response>
    /// <response code="404">If user does not exist</response>
    /// <response code="500">If there was an error</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
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
    /// <summary>
    /// This method is used to delete a user.
    /// </summary>
    /// <param name="user">User instance to be deleted</param>
    /// <returns>Appropriate status code response</returns>
    /// <response code="200">If everything is correct</response>
    /// <response code="404">If user does not exist</response>
    /// <response code="500">If there was an error</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] UserModel user) {
        var userInDb = await userDb.Users.FindAsync(user.Name);
        if (userInDb is null) return NotFound();
        userDb.Users.Remove(userInDb);
        await userDb.SaveChangesAsync();
        return Ok();
    }
}