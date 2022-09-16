using ClinicService.Data;
using ClinicService.Data.Entitys;
using ClinicServiceProtos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static ClinicServiceProtos.PetService;

namespace ClinicService.Services.Implementation
{
    public class PetService : PetServiceBase
    {
        private readonly ClinicServiceDbContext _dbContext;
        private readonly ILogger<PetService> _logger;

        public PetService(ClinicServiceDbContext dbContext,
            ILogger<PetService> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public override Task<ClinicServiceProtos.CreatePetResponse> CreatePet(ClinicServiceProtos.CreatePetRequest request, ServerCallContext context)
        {
            _logger.LogInformation("ClinicClientService CreatePet");

            var pet = new Pet
            {
                ClientId = request.ClientId,
                Name = request.Name,
                Birthday = request.Birthday.ToDateTime()
            };

            _dbContext.Add(pet);
            _dbContext.SaveChanges();

            var response = new CreatePetResponse
            {
                PetId = pet.PetId
            };

            return Task.FromResult(response);
        }

        public override Task<ClinicServiceProtos.GetPetsResponse> GetPets(ClinicServiceProtos.GetPetsRequest request, ServerCallContext context)
        {
            _logger.LogInformation("ClinicClientService GetPets");

            var response = new GetPetsResponse();
            response.Pets.AddRange(_dbContext.Pets.Select(pet => new PetResponse
            {
                PetId = pet.PetId,
                ClientId = pet.ClientId,
                Name = pet.Name,
                Birthday = pet.Birthday.ToUniversalTime().ToTimestamp()
            }).ToList());

            return Task.FromResult(response);
        }
    }
}
