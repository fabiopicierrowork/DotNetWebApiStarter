﻿namespace DotNetWebApiStarter.Models
{
    public class User
    {
        public int Id { get; set; }
        public int IdRole { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
