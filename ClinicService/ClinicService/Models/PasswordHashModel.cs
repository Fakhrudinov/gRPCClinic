namespace ClinicService.Models
{
    public class PasswordHashModel
    {
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
    }
}
