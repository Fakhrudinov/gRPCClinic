namespace ClinicService.Models.Responses
{
    public class AuthenticationResponse
    {
        public AuthenticationStatusEnum Status { get; set; }

        public SessionContext SessionContext { get; set; }
    }
}
