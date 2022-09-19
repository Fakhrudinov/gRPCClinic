using ClinicService.Data;
using ClinicService.Data.Entitys;
using Microsoft.EntityFrameworkCore;

namespace ClinicService.Services.Implementation
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ClinicServiceDbContext _dbContext;
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(ClinicServiceDbContext dbContext, ILogger<AccountRepository> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<bool> GetIsUserExistByEMail(string eMail)
        {
            _logger.LogInformation($"Repository GetIsUserExistByEMail {eMail}");

            Account result = await _dbContext.Accounts.FirstOrDefaultAsync(user => user.EMail == eMail);
            if (result is null)
            {
                return false;
            }
            return true;
        }

        public async Task<int> SaveNewUserInDataBase(Account newDBAccount)
        {
            _logger.LogInformation($"Repository SaveNewUserInDataBase {newDBAccount.EMail}");

            await _dbContext.Accounts.AddAsync(newDBAccount);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Repository New client id is {newDBAccount.AccountId}");

            return newDBAccount.AccountId;
        }
    }
}
