using System;

namespace RACEDAY_API.Models
{
    public class USER
{

        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string Role { get; set; }

        public string PhoneNumber { get; set; }

        public string ProfilePictureUrl { get; set; }

    }
}
