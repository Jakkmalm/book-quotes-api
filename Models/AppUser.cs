namespace BookQuotesApi.Models
{
    public class AppUser
    {
        public int Id { get; set; }  
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;  // HASH!
        public string? Role { get; set; }  // Till framtida...
    }
}
