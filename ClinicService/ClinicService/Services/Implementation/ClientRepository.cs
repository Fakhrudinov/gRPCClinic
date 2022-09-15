using ClinicService.Data.Entitys;
using ClinicService.Data;

namespace ClinicService.Services.Implementation
{
    public class ClientRepository : IClientRepository
    {

        #region Serives

        private readonly ClinicServiceDbContext _dbContext;
        private readonly ILogger<ClientRepository> _logger;

        #endregion

        #region Constructors

        public ClientRepository(ClinicServiceDbContext dbContext,
            ILogger<ClientRepository> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        #endregion

        public int Add(Client item)
        {
            _logger.LogInformation($"Repository Add {item.FirstName} {item.Surname} {item.Document}");

            _dbContext.Clients.Add(item);
            _dbContext.SaveChanges();

            _logger.LogInformation($"Repository New client id is {item.ClientId}");

            return item.ClientId;
        }

        public void Delete(Client item)
        {
            if (item == null)
            {
                _logger.LogWarning($"Repository Delete called with null client");
                throw new NullReferenceException();
            }                

            _logger.LogInformation($"Repository Delete {item.FirstName} {item.Surname} {item.Document}");

            Delete(item.ClientId);
        }

        public void Delete(int id)
        {
            _logger.LogInformation($"Repository Delete by id {id}");

            var client = GetById(id);
            if (client == null)
            {
                _logger.LogWarning($"Repository Delete by id {id} failed - client not found");
                throw new KeyNotFoundException();
            }

            _dbContext.Remove(client);
            _dbContext.SaveChanges();
        }

        public IList<Client> GetAll()
        {
            _logger.LogInformation($"Repository GetAll clients");

            return _dbContext.Clients.ToList();
        }

        public Client? GetById(int id)
        {
            _logger.LogInformation($"Repository GetById {id}");

            return _dbContext.Clients.FirstOrDefault(client => client.ClientId == id);
        }

        public void Update(Client item)
        {          
            if (item == null)
            {
                _logger.LogWarning($"Repository Update failed - called with null client");
                throw new NullReferenceException();
            }

            _logger.LogInformation($"Repository Update {item.ClientId} {item.FirstName} {item.Surname} {item.Document}");

            var client = GetById(item.ClientId);
            if (client == null)
            {
                _logger.LogWarning($"Repository Update failed - client not found by id {item.ClientId}");
                throw new KeyNotFoundException();
            }                

            client.Surname = item.Surname;
            client.FirstName = item.FirstName;
            client.Patronymic = item.Patronymic;
            client.Document = item.Document;

            _dbContext.Update(client);
            _dbContext.SaveChanges();
        }
    }
}
