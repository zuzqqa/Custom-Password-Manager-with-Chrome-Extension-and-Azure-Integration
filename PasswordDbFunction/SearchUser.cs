using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PasswordDbFunction.DbService;
using PasswordDbFunction.Models;

namespace PasswordDbFunction;

public class SearchUser(ILogger<SearchUser> logger, UserDbContext userDb) {
    [Function("SearchUser")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<UserToValidate>(requestBody);

        if (data is null)
            return new BadRequestObjectResult("Please pass a name and password in the request body");

        var userFromDatabase = userDb.Users.FirstOrDefault(u => u.Name == data.Name);
        if (userFromDatabase is null)
            return new BadRequestObjectResult("User not found");

        if (BCrypt.Net.BCrypt.Verify(data.Password, userFromDatabase.Password)) 
            return new OkObjectResult("User found");
        
        return new BadRequestObjectResult("Invalid password");
    }
}