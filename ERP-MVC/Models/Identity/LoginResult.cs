namespace ERP_MVC.Models.Identity
{
    public class LoginResult
    {
        public string UserName { get; set; }
        public TokenResult TokenResult { get; set; }

        public List<AppClaim> Claims { get; set; }
    }

    public class TokenResult
    {
        public string Token { get; set; }
        public DateTime TokenExpiryTime { get; set; }

        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }

    public class AppClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
