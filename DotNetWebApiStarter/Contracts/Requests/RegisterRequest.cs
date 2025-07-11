namespace DotNetWebApiStarter.Contracts.Requests
{
    public class RegisterRequest
    {
        public int IdRole { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
