namespace FribergCarRental.Core.Classes
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public DateTime Expires { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
    }
}
