using Microsoft.AspNetCore.Mvc;
using PasswordApi.Database;
using PasswordApi.Models;
using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

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
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception e) {
            logger.LogError(e.Message, user);
            return BadRequest(e.Message);
        }
    }

    // Post: /<controller>/password/save
    /// <summary>
    /// Saves a password to the key vault.
    /// </summary>
    /// <param name="password">Username and plain password to save</param>
    /// <returns>Appropriate status code</returns>
    /// <response code="201">If password was saved</response>
    /// <response code="500">If there was an error</response>
    [HttpPost("password/save")]
    public async Task<IActionResult> SavePassword([FromBody] PasswordModel password) {
        var user = await userDb.Users.FindAsync(password.Username);
        if (user is null) 
            return NotFound();

        logger.LogInformation($"Saving password for {password.Username} at {password.SiteAddress}.");
        // Save password to key vault

        try {
            var secretName = password.Username + password.SiteAddress.ToUpper();
            var secretValue = password.PlainPassword;

            const string keyVaultName = "CyberProjektKV";
            const string kvUri = $"https://{keyVaultName}.vault.azure.net";

            var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

            logger.LogInformation(
                $"Creating a secret in {keyVaultName} called '{secretName}' with the value '{secretValue}' ...");
            await client.SetSecretAsync(secretName, secretValue);

            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return BadRequest(e.Message);
        }
    }

    // POST: /<controller>/password/get
    /// <summary>
    /// 
    /// </summary>
    /// <param name="loggedUser"></param>
    /// <returns></returns>
    [HttpPost("password/get")]
    public async Task<IActionResult> GetPassword([FromBody] UserModel loggedUser) {
        var userInDb = await userDb.Users.FindAsync(loggedUser.Name);

        if (userInDb is null)
            return NotFound();

        //if (!BCrypt.Net.BCrypt.Verify(userInDb.Password, loggedUser.Password)) 
        //    return Unauthorized();

        // Get password from key vault
        const string keyVaultName = "CyberProjektKV";
        const string kvUri = $"https://{keyVaultName}.vault.azure.net";

        var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
        var secrets = new List<(string, string)>();

        await foreach (var secretProperties in client.GetPropertiesOfSecretsAsync()) {
            if (!secretProperties.Name.Contains(loggedUser.Name)) 
                continue;
            
            var secret = await client.GetSecretAsync(secretProperties.Name);
            secrets.Add((secret.Value.Name, secret.Value.Value));
        }

        // Return all passwords for the user
        return Ok(secrets);

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
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)] // No content
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound, Type = typeof(void))] // No content
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized, Type = typeof(void))] // No content
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserToValidate user) {
        var userInDb = await userDb.Users.FindAsync(user.Name);
        if (userInDb is null) 
            return NotFound();
        if (BCrypt.Net.BCrypt.Verify(user.Password, userInDb.Password)) 
            return Ok();

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