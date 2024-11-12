using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.Buffers;
using DoeTest.Models;

namespace DoeTest.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly string _jsonFilePath = "users.json";

        [HttpGet("birthdays")]
        public async Task<IActionResult> GetUsersWithBirthdaysInRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate == default || endDate == default)
            {
                return BadRequest("Both startDate and endDate must be provided.");
            }

            var usersInDateRange = new List<ApplicationUser>();

            using (var stream = new FileStream(_jsonFilePath, FileMode.Open, FileAccess.Read))
            {
                await foreach (var user in JsonSerializer.DeserializeAsyncEnumerable<ApplicationUser>(stream))
                {
                    if (user != null && DateTime.TryParse(user.DateOfBirthString, out DateTime dateOfBirth))
                    {
                        if (dateOfBirth >= startDate && dateOfBirth <= endDate)
                        {
                            usersInDateRange.Add(user);
                        }
                    }
                }
            }

            return Ok(usersInDateRange);
        }
    }
}
