using ClinicService.Data.Entitys;
using ClinicService.Models;
using ClinicService.Models.Requests;
using ClinicService.Services;
using ClinicService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ClinicService.Controllers
{
    [Authorize]
    //новых пользователей в нашем случае может создавать только авторизованный пользователь.
    //Хорошо бы еще с только ролью админа.
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accRepository;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountRepository accRepository, ILogger<AccountController> logger)
        {
            _logger = logger;
            _accRepository = accRepository;
        }

        [HttpPost("CreateNew")]
        public async Task<IActionResult> Register(CreateNewAccountRequest model)
        {
            _logger.LogInformation($"Register new user email='{model.EMail}' Name={model.FirstName} LastName={model.LastName}");

            bool exist = await _accRepository.GetIsUserExistByEMail(model.EMail);

            if (!exist)
            {
                _logger.LogDebug($"Ok, user not found {model.EMail}, set it to DB");
                Account newDBAccount = new Account();
                newDBAccount.EMail = model.EMail;
                newDBAccount.FirstName = model.FirstName;
                newDBAccount.LastName = model.LastName;
                newDBAccount.SecondName = model.SecondName;

                // generate pass hash
                PasswordHashModel passHash = PasswordUtils.CreatePasswordHash(model.Password);
                newDBAccount.PasswordSalt = passHash.PasswordSalt;
                newDBAccount.PasswordHash = passHash.PasswordHash;

                newDBAccount.Locked = false;

                // send user in dataBase
                int newUserId = await _accRepository.SaveNewUserInDataBase(newDBAccount);

                _logger.LogInformation($"Register new user email='{model.EMail}' success with new id {newUserId}");
                return Ok(newUserId);
            }
            else
            {
                _logger.LogInformation($"Adding failed - user {model.EMail} already exist ");
                return Ok($"Adding failed - user {model.EMail} already exist");
            }
        }
    }
}
