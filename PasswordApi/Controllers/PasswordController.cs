using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;

namespace PasswordApi.Controllers;

/// <summary>
/// This controller generates a random password.
/// </summary>
[ApiController]
[Route("[controller]/generate")]
public class PasswordController : Controller {
    private const int DefaultLength = 16;

    private const int AsciiStart = 33;
    private const int AsciiEnd = 122;

    private static readonly char[] Ascii = Enumerable.Range(AsciiStart,
            AsciiEnd - AsciiStart + 1)
        .Select(Convert.ToChar)
        .ToArray();

    // GET /password/generate
    /// <summary>
    /// Generates a secure, random password of DefaultLength.
    /// </summary>
    /// <returns>Cryptographically secure password</returns>
    /// <responce code="200">Returns a secure password</responce>
    /// <response code="500">If there was an error</response>
    [HttpGet("")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Generate() => Ok(RandomNumberGenerator.GetString(Ascii, DefaultLength));

    // GET /password/generate/{length}
    /// <summary>
    /// Generates a secure, random password of the specified length.
    /// </summary>
    /// <param name="length">Character length. Must be at least 8, and not above 255.</param>
    /// <returns>Cryptographically secure password</returns>
    /// <response code="200">Returns a secure password</response>
    /// <response code="418">If the length is invalid, you are a teapot</response>
    /// <response code="500">If there was an error</response>
    [HttpGet("{length:int}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status418ImATeapot)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public IActionResult Generate(int length) =>
        length is < 8 or > 255
            ? StatusCode(StatusCodes.Status418ImATeapot, "Length must be between 8 and 255.")
            : Ok(RandomNumberGenerator.GetString(Ascii, length));
}