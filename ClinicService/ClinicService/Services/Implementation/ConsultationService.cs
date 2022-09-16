using ClinicService.Data;
using ClinicService.Data.Entitys;
using Google.Protobuf.WellKnownTypes;
using static ClinicServiceProtos.ConsultationService;
using ClinicServiceProtos;
using Grpc.Core;

namespace ClinicService.Services.Implementation
{
    public class ConsultationService : ConsultationServiceBase
    {
        private readonly ClinicServiceDbContext _dbContext;
        private readonly ILogger<ConsultationService> _logger;

        public ConsultationService(ClinicServiceDbContext dbContext, ILogger<ConsultationService> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public override Task<ClinicServiceProtos.CreateConsultationResponse> CreateConsultation(ClinicServiceProtos.CreateConsultationRequest request, ServerCallContext context)
        {
            _logger.LogInformation("ClinicClientService CreateConsultation");

            var consultation = new Consultation
            {
                ClientId = request.ClientId,
                PetId = request.PetId,
                ConsultationDate = request.ConsultationDate.ToDateTime(),
                Description = request.Description
            };

            _dbContext.Add(consultation);
            _dbContext.SaveChanges();

            var response = new CreateConsultationResponse
            {
                ConsultationId = consultation.ConsultationId
            };

            return Task.FromResult(response);
        }

        public override Task<ClinicServiceProtos.GetConsultationsResponse> GetConsultations(ClinicServiceProtos.GetConsultationsRequest request, ServerCallContext context)
        {
            _logger.LogInformation("ClinicClientService GetConsultations");

            var response = new GetConsultationsResponse();
            response.Consultations.AddRange(_dbContext.Consultations.Select(consultation => new ConsultationResponse
            {
                ConsultationId = consultation.ConsultationId,
                ClientId = consultation.ClientId,
                PetId = consultation.PetId,
                ConsultationDate = consultation.ConsultationDate.ToUniversalTime().ToTimestamp(),
                Description = consultation.Description,
            }).ToList());

            return Task.FromResult(response);
        }
    }
}
