namespace EBookApp.Models
{
    public class UserViewModel
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public required string UserName { get; set; }
        public bool IsAdmin { get; set; }
    }
}
