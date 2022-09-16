using ClinicService.Data.Entitys;
using ClinicService.Data;

namespace ClinicService.Services.Implementation
{
    public class PetRepository : IPetRepository
    {

        #region Serives

        private readonly ClinicServiceDbContext _dbContext;
        private readonly ILogger<PetRepository> _logger;

        #endregion

        #region Constructors

        public PetRepository(ClinicServiceDbContext dbContext,
            ILogger<PetRepository> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        #endregion

        public int Add(Pet item)
        {
            _logger.LogInformation($"Repository Add {item.Name} {item.ClientId} {item.Birthday}");

            _dbContext.Pets.Add(item);
            _dbContext.SaveChanges();

            _logger.LogInformation($"Repository New client id is {item.PetId}");

            return item.PetId;
        }

        public void Delete(Pet item)
        {
            if (item is null)
            {
                _logger.LogWarning($"Repository Delete called with null pet");
                throw new NullReferenceException();
            }

            _logger.LogInformation($"Repository Delete {item.Name} {item.PetId}");

            Delete(item.PetId);
        }

        public void Delete(int id)
        {
            _logger.LogInformation($"Repository Delete by id {id}");

            var pet = GetById(id);

            if (pet is null)
            {
                _logger.LogWarning($"Repository Delete by id {id} failed - pet not found");
                throw new KeyNotFoundException();
            }

            _dbContext.Remove(pet);
            _dbContext.SaveChanges();
        }

        public IList<Pet> GetAll()
        {
            _logger.LogInformation($"Repository GetAll pets");
            return _dbContext.Pets.ToList();
        }

        public Pet? GetById(int id)
        {
            _logger.LogInformation($"Repository GetById {id}");
            return _dbContext.Pets.FirstOrDefault(pet => pet.PetId == id);
        }

        public void Update(Pet item)
        {
            if (item is null)
            {
                _logger.LogWarning($"Repository Update failed - called with null pet");
                throw new NullReferenceException();
            }

            _logger.LogInformation($"Repository Update {item.Name} {item.PetId} {item.ClientId} {item.Birthday}");

            var pet = GetById(item.PetId);

            if (pet is null)
            {
                _logger.LogWarning($"Repository Update failed - pet not found by id {item.PetId}");
                throw new KeyNotFoundException();
            }

            pet.ClientId = item.ClientId;
            pet.Name = item.Name;
            pet.Birthday = item.Birthday;

            _dbContext.Update(pet);
            _dbContext.SaveChanges();
        }
    }
}
