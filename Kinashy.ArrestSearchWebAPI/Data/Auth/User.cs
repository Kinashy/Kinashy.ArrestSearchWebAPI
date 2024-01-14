namespace Kinashy.ArrestSearchWebAPI.Data.Auth
{
    public record UserDto(string userName, string password);
    public record UserModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
