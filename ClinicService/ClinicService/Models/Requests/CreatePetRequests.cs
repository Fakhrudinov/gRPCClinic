namespace ClinicService.Models.Requests
{
    public class CreatePetRequests
    {
        public int ClientId { get; set; }
        public string? Name { get; set; }
        public DateTime Birthday { get; set; }
    }
}
