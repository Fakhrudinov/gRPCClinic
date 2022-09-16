using ClinicService.Data.Entitys;
using ClinicService.Data;

namespace ClinicService.Services.Implementation
{
    public class ConsultationRepository : IConsultationRepository
    {
        private readonly ClinicServiceDbContext _dbContext;
        private readonly ILogger<ConsultationRepository> _logger;

        public ConsultationRepository(ClinicServiceDbContext dbContext,
            ILogger<ConsultationRepository> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public int Add(Consultation item)
        {
            _logger.LogInformation($"Repository Add {item.PetId} {item.ClientId} {item.ConsultationDate} {item.Description}");

            _dbContext.Consultations.Add(item);
            _dbContext.SaveChanges();

            _logger.LogInformation($"Repository New consult id is {item.ConsultationId}");

            return item.ConsultationId;
        }

        public void Delete(Consultation item)
        {
            if (item is null)
            {
                _logger.LogWarning($"Repository Delete called with null consult");
                throw new NullReferenceException();
            }

            _logger.LogInformation($"Repository Delete {item.ConsultationId}");

            Delete(item.ConsultationId);
        }

        public void Delete(int id)
        {
            _logger.LogInformation($"Repository Delete by id {id}");
            var consultation = GetById(id);

            if (consultation is null)
            {
                _logger.LogWarning($"Repository Delete by id {id} failed - consult not found");
                throw new KeyNotFoundException();
            }

            _dbContext.Remove(consultation);
            _dbContext.SaveChanges();
        }

        public IList<Consultation> GetAll()
        {
            _logger.LogInformation($"Repository GetAll consults");
            return _dbContext.Consultations.ToList();
        }

        public Consultation? GetById(int id)
        {
            _logger.LogInformation($"Repository GetById {id}");
            return _dbContext.Consultations.FirstOrDefault(consultation => consultation.ConsultationId == id);
        }

        public void Update(Consultation item)
        {
            if (item is null)
            {
                _logger.LogWarning($"Repository Update failed - called with null consult");
                throw new NullReferenceException();
            }

            _logger.LogInformation($"Repository Update {item.ConsultationId} {item.PetId} {item.ClientId} {item.ConsultationDate} {item.Description}");
            var consultation = GetById(item.ConsultationId);

            if (consultation is null)
            {
                _logger.LogWarning($"Repository Update failed - consult not found by id {item.ConsultationId}");
                throw new KeyNotFoundException();
            }

            consultation.ConsultationId = item.ConsultationId;
            consultation.ClientId = item.ClientId;
            consultation.PetId = item.PetId;
            consultation.ConsultationDate = item.ConsultationDate;
            consultation.Description = item.Description;

            _dbContext.Update(consultation);
            _dbContext.SaveChanges();
        }
    }
}
