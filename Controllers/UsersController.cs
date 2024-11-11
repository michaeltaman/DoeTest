using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DoeTest.Models;

namespace DoeTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly string _jsonFilePath = "users.json"; // Path to your JSON file

        [HttpGet("birthdays")]
        public async Task<IActionResult> GetUsersWithBirthdaysInRange([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            // Validate that both startDate and endDate are provided and valid
            if (!startDate.HasValue || !endDate.HasValue)
            {
                return BadRequest("Both startDate and endDate must be provided.");
            }

            // Validate that startDate is not after endDate
            if (startDate > endDate)
            {
                return BadRequest("The startDate cannot be after the endDate.");
            }

            // Read the JSON file
            var jsonData = await System.IO.File.ReadAllTextAsync(_jsonFilePath);
            if (string.IsNullOrWhiteSpace(jsonData))
            {
                return BadRequest("The JSON data is empty or invalid.");
            }

            // Deserialize the JSON content
            List<ApplicationUser> users;
            try
            {
                users = JsonSerializer.Deserialize<List<ApplicationUser>>(jsonData) ?? new List<ApplicationUser>();
            }
            catch (JsonException ex)
            {
                return BadRequest("Error deserializing JSON data: " + ex.Message);
            }

            // Convert startDate and endDate to only month and day (ignoring year)
            var startMonthDay = new DateTime(1, startDate.Value.Month, startDate.Value.Day);
            var endMonthDay = new DateTime(1, endDate.Value.Month, endDate.Value.Day);
            bool wrapsAround = startMonthDay > endMonthDay;

            // Filter users by birthday range
            var usersInDateRange = users.Where(user =>
            {
                if (!DateTime.TryParse(user.DateOfBirthString, out DateTime dateOfBirth))
                {
                    return false;
                }

                var birthdayMonthDay = new DateTime(1, dateOfBirth.Month, dateOfBirth.Day);

                if (wrapsAround)
                {
                    return birthdayMonthDay >= startMonthDay || birthdayMonthDay <= endMonthDay;
                }
                else
                {
                    return birthdayMonthDay >= startMonthDay && birthdayMonthDay <= endMonthDay;
                }
            }).ToList();

            return Ok(usersInDateRange);
        }

    }
}
