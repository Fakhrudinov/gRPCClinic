using ClinicService.Data.Entitys;
using ClinicService.Data;
using Grpc.Core;
using static ClinicServiceProtos.ClinicClientService;
using ClinicServiceProtos;
using Microsoft.AspNetCore.Authorization;

namespace ClinicService.Services.Implementation
{
    [Authorize]
    public class ClinicClientService : ClinicClientServiceBase
    {
        private readonly ClinicServiceDbContext _dbContext;
        private readonly ILogger<ClinicClientService> _logger;

        public ClinicClientService(ClinicServiceDbContext dbContext, ILogger<ClinicClientService> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public override Task<ClinicServiceProtos.CreateClientResponse> CreateClient(ClinicServiceProtos.CreateClientRequest request, ServerCallContext context)
        {
            _logger.LogInformation("ClinicClientService CreateClient");

            var client = new Client
            {
                Document = request.Document,
                Surname = request.Surname,
                FirstName = request.FirstName,
                Patronymic = request.Patronymic
            };

            _dbContext.Clients.Add(client);

            _dbContext.SaveChanges();

            var response = new CreateClientResponse
            {
                ClientId = client.ClientId
            };

            return Task.FromResult(response);
        }

        public override Task<ClinicServiceProtos.GetClientsResponse> GetClients(ClinicServiceProtos.GetClientsRequest request, ServerCallContext context)
        {
            _logger.LogInformation("ClinicClientService GetClients");

            var response = new GetClientsResponse();

            response.Clients.AddRange(_dbContext.Clients.Select(client => new ClientResponse
            {
                ClientId = client.ClientId,
                Document = client.Document,
                FirstName = client.FirstName,
                Patronymic = client.Patronymic,
                Surname = client.Surname
            }).ToList());

            return Task.FromResult(response);
        }
    }
}
