namespace PSK2025.Data.Seed.Models
{
    public class UserSeedData
    {
        public List<UserAccount> Users { get; set; } = new List<UserAccount>();
    }

    public class UserAccount
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer";
    }
}