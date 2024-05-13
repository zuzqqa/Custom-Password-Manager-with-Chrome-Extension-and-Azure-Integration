using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using PasswordDbFunction.DbService;

namespace PasswordDbFunction;

public class GetAllUsers(ILogger<GetAllUsers> logger, UserDbContext userDb) {
    [Function("GetAllUsers")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req) {
        logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult(userDb.Users.ToList());
    }
}