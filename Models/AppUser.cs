namespace BookQuotesApi.Models
{
    public class AppUser
    {
        public int Id { get; set; }  // Primärnyckel
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;  // Vi sparar inte plaintext!
        public string? Role { get; set; }  // Valfritt, t.ex. "User", "Admin"
    }
}
