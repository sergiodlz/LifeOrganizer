namespace LifeOrganizer.Business.DTOs.Auth
{
    public class ChangePasswordDto
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
