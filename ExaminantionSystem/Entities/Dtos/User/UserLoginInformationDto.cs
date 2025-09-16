namespace ExaminantionSystem.Entities.Dtos.User
{
    public class UserLoginInformationDto
    {
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public int UserId { get; set; }
    }
}
