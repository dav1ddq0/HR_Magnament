using HR_API.Models;

namespace HR_API.DTOs
{
    public record AddUserRequest
    {

        public string Name { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? PersonalAddress { get; set; }
        public string? Phone { get; set; }
        public DateTime WorkingStartDate { get; set; }
        public double StartedSalary { get; set; }
        public string[] Roles { get; set; } = Array.Empty<string>();
    }
}
