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
            if (startDate == default || endDate == default)
            {
                return BadRequest("Both startDate and endDate must be provided.");
            }

            var jsonData = await System.IO.File.ReadAllTextAsync(_jsonFilePath);
            if (string.IsNullOrWhiteSpace(jsonData))
            {
                return BadRequest("The JSON data is empty or invalid.");
            }

            List<ApplicationUser> users;
            try
            {
                users = JsonSerializer.Deserialize<List<ApplicationUser>>(jsonData) ?? new List<ApplicationUser>();
            }
            catch (JsonException ex)
            {
                return BadRequest("Error deserializing JSON data: " + ex.Message);
            }

            var usersInDateRange = users.Where(user =>
            {
                if (!DateTime.TryParse(user.DateOfBirthString, out DateTime dateOfBirth))
                {
                    return false;
                }

                return dateOfBirth >= startDate && dateOfBirth <= endDate;
            }).ToList();

            return Ok(usersInDateRange);
        }
    }
}
