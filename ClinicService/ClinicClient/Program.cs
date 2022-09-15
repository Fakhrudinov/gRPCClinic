using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using static ClinicServiceProtos.ClinicClientService;
using static ClinicServiceProtos.ConsultationService;
using static ClinicServiceProtos.PetService;

namespace ClinicClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // для работы с незащищенным соединением
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", 
                true);
            
            var channel = GrpcChannel.ForAddress("http://localhost:5001");
            ClinicClientServiceClient client = new ClinicClientServiceClient(channel);
            PetServiceClient pet = new PetServiceClient(channel);
            ConsultationServiceClient consultation = new ConsultationServiceClient(channel);

            // clients --------------------------------------------------------------
            var createClientResponse = client.CreateClient(new ClinicServiceProtos.CreateClientRequest
            {
                Document = "44qrswer",
                FirstName = "444qwerq123412341324wer",
                Surname = "s4444aasdfasdfa",
                Patronymic = "segrw4rebt34"
            });

            Console.WriteLine($"Client ({createClientResponse.ClientId}) created successfully.");

            var getClientsResponse = client.GetClients(new ClinicServiceProtos.GetClientsRequest());

            Console.WriteLine("Clients:");
            Console.WriteLine("========\n");
            int idClientForConsultation = 1;
            foreach (var clientObj in getClientsResponse.Clients)
            {
                Console.WriteLine($"{clientObj.ClientId} {clientObj.Document} >> {clientObj.Surname} {clientObj.FirstName}");
                idClientForConsultation = clientObj.ClientId;
            }

            // pets --------------------------------------------------------------
            var createPetResponse = pet.CreatePet(new ClinicServiceProtos.CreatePetRequest
            {
                ClientId = 1,
                Name = "КошкаКошка",
                Birthday = DateTime.UtcNow.ToTimestamp()
            });

            Console.WriteLine($"Pet ({createPetResponse.PetId}) created successfully.");

            var getPetsResponse = pet.GetPets(new ClinicServiceProtos.GetPetsRequest());

            Console.WriteLine("Pets:");
            Console.WriteLine("========\n");
            int idPetForConsultation = 1;
            foreach (var petDto in getPetsResponse.Pets)
            {
                Console.WriteLine($"({petDto.PetId} {petDto.ClientId} {petDto.Name}) {petDto.Birthday}");
                idPetForConsultation = petDto.PetId;
            }


            // consult --------------------------------------------------------------
            var createConsultationResponse = consultation.CreateConsultation(new ClinicServiceProtos.CreateConsultationRequest
            {
                ClientId = idClientForConsultation,
                PetId = idPetForConsultation,
                ConsultationDate = DateTime.UtcNow.ToTimestamp(),
                Description = "почесать пузо",
            });

            Console.WriteLine($"Consultation ({createConsultationResponse.ConsultationId}) created successfully.");

            var getConsultationsResponse = consultation.GetConsultations(new ClinicServiceProtos.GetConsultationsRequest());

            Console.WriteLine("Consultations:");
            Console.WriteLine("========\n");
            foreach (var consultationDto in getConsultationsResponse.Consultations)
            {
                Console.WriteLine($"({consultationDto.ConsultationId} client={consultationDto.ClientId} pet={consultationDto.PetId} {consultationDto.ConsultationDate} {consultationDto.Description})");
            }


            Console.ReadKey();
        }
    }
}