namespace HR_API.DTOs
{
    public class UpdateUserRequest
    {
        public string? Name { get; set; }
        
        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? PersonalAddress { get; set; }

        public string? Phone { get; set; }

        public string[]? Roles { get; set; }
    }
}
