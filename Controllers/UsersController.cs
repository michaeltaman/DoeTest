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
        public async Task<IActionResult> GetUsersWithBirthdaysInRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            // Read the JSON file
            var jsonData = await System.IO.File.ReadAllTextAsync(_jsonFilePath);
            if (string.IsNullOrWhiteSpace(jsonData))
            {
                Console.WriteLine("The JSON data is empty or invalid.");
                return BadRequest("The JSON data is empty or invalid.");
            }

            // Deserialize the JSON content
            List<ApplicationUser> users;
            try
            {
                users = JsonSerializer.Deserialize<List<ApplicationUser>>(jsonData) ?? new List<ApplicationUser>();
                Console.WriteLine($"Deserialized {users.Count} users from JSON.");
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Error deserializing JSON data: " + ex.Message);
                return BadRequest("Error deserializing JSON data: " + ex.Message);
            }

            // Log deserialized user data for debugging
            foreach (var user in users)
            {
                Console.WriteLine($"User: {user.FirstName} {user.LastName}, DateOfBirthString: {user.DateOfBirthString}");
            }

            // Convert startDate and endDate to only month and day (ignoring year)
            var startMonthDay = new DateTime(1, startDate.Month, startDate.Day);
            var endMonthDay = new DateTime(1, endDate.Month, endDate.Day);
            bool crossesYearBoundary = startMonthDay > endMonthDay;

            Console.WriteLine($"Start Date (Month/Day): {startMonthDay.ToShortDateString()}, End Date (Month/Day): {endMonthDay.ToShortDateString()}, Crosses Year Boundary: {crossesYearBoundary}");

            // Filter users by birthday range (using DateOfBirthString parsed manually)
            var usersInDateRange = users.Where(user =>
            {
                // Parse DateOfBirthString into DateTime
                if (!DateTime.TryParse(user.DateOfBirthString, out DateTime dateOfBirth))
                {
                    Console.WriteLine($"Failed to parse DateOfBirthString for user {user.FirstName} {user.LastName}");
                    return false; // Skip if parsing fails
                }

                // Get month and day only for comparison
                var birthdayMonthDay = new DateTime(1, dateOfBirth.Month, dateOfBirth.Day);
                Console.WriteLine($"User: {user.FirstName} {user.LastName}, Birthday (Month/Day): {birthdayMonthDay.ToShortDateString()}");

                // Check if it falls within the date range
                if (crossesYearBoundary)
                {
                    return birthdayMonthDay >= startMonthDay || birthdayMonthDay <= endMonthDay;
                }
                else
                {
                    return birthdayMonthDay >= startMonthDay && birthdayMonthDay <= endMonthDay;
                }
            }).ToList();

            Console.WriteLine($"Found {usersInDateRange.Count} users with birthdays in range.");
            return Ok(usersInDateRange);
        }
    }
}
