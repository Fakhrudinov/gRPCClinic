using ClinicService.Data.Entitys;

namespace ClinicService.Services
{
    public interface IAccountRepository
    {
        Task<bool> GetIsUserExistByEMail(string eMail);
        Task<int> SaveNewUserInDataBase(Account newDBAccount);
    }
}
