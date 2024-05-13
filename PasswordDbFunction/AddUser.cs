using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PasswordDbFunction.DbService;
using PasswordDbFunction.Models;

namespace PasswordDbFunction;

public class AddUser(ILogger<AddUser> logger, UserDbContext userDb) {
    [Function("AddUser")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req) {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<UserModel>(requestBody);

        if (data is null)
            return new BadRequestObjectResult("Please pass a name and password in the request body");

        try {
            await userDb.Users.AddAsync(data);
            await userDb.SaveChangesAsync();

            return new OkObjectResult($"User {data.Name} saved.");
        }
        catch (Exception e) {
            logger.LogError(e, e.Message);
            return new BadRequestObjectResult(e.Message);
        }
    }
}