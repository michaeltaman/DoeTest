// File: Models/ApplicationUser.cs
using System;
using System.Text.Json.Serialization;

namespace DoeTest.Models
{
    public class ApplicationUser
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("dateOfBirth")]
        public string DateOfBirthString { get; set; } = string.Empty;
    }
}
