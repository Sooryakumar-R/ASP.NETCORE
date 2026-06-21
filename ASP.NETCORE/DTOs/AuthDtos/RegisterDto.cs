namespace ASP.NETCORE.DTOs.AuthDtos
{
    public class RegisterDto
    {
       
        public string Username { get; set; }

        public string Password { get; set; }

        public string Role { get; set; } = "User";
        
    }
}
