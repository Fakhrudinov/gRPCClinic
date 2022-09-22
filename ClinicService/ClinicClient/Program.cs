using ClinicServiceProtos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using static ClinicServiceProtos.AuthenticateService;
using static ClinicServiceProtos.ClinicClientService;
using static ClinicServiceProtos.ConsultationService;
using static ClinicServiceProtos.PetService;

namespace ClinicClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
            // использовалось для работы с незащищенным соединением
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", 
                true);
            GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:5001");// N.B. -----------> http
            */

            GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001");// N.B. -----------> https

            // logon --------------------------------------------------------------
            AuthenticateServiceClient authenticateServiceClient = new AuthenticateServiceClient(channel);
            AuthenticationResponse authenticationResponse = authenticateServiceClient.Login(new AuthenticationRequest
            {
                UserName = "tttt@ssss.ru",
                Password = "12345"
            });

            if (authenticationResponse.Status != 0)
            {
                Console.WriteLine("Authentication error.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Session token: {authenticationResponse.SessionContext.SessionToken}");

            // add token to requests --------------------------------------------------------------
            CallCredentials callCredentials = CallCredentials.FromInterceptor((c, m) =>
            {
                m.Add(
                    "Authorization",
                    $"Bearer {authenticationResponse.SessionContext.SessionToken}");

                return Task.CompletedTask;
            });


            GrpcChannel protectedChannel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), callCredentials)
            });



            /*
             * обычный какнал только в случае незащищенного соединения
            ClinicClientServiceClient client = new ClinicClientServiceClient(channel);
            PetServiceClient pet = new PetServiceClient(channel);
            ConsultationServiceClient consultation = new ConsultationServiceClient(channel);
            */

            ClinicClientServiceClient client = new ClinicClientServiceClient(protectedChannel);
            PetServiceClient pet = new PetServiceClient(protectedChannel);
            ConsultationServiceClient consultation = new ConsultationServiceClient(protectedChannel);

            

            // clients --------------------------------------------------------------
            CreateClientResponse createClientResponse = client.CreateClient(new ClinicServiceProtos.CreateClientRequest
            {
                Document = "55qrswer",
                FirstName = "55555qwerq123412341324wer",
                Surname = "s55555aasdfasdfa",
                Patronymic = "5555segrw4rebt34"
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
                ClientId = 2,
                Name = "Мышка",
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