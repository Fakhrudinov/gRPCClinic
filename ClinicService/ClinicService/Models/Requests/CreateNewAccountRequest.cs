namespace ClinicService.Models.Requests
{
    public class CreateNewAccountRequest
    {
        public string EMail { get; set; }// used as login
        public string Password { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SecondName { get; set; }
    }
}
