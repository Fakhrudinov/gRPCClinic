using ClinicService.Models;
using ClinicService.Models.Requests;
using ClinicService.Models.Responses;
using ClinicService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace ClinicService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        #region Services

        private readonly IAuthenticateService _authenticateService;
        private readonly ILogger<AuthenticateController> _logger;

        #endregion

        #region Constructors

        public AuthenticateController(IAuthenticateService authenticateService, ILogger<AuthenticateController> logger)
        {
            _authenticateService = authenticateService;
            _logger=logger;
        }

        #endregion

        #region Public Methods

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<AuthenticationResponse> Login([FromBody] AuthenticationRequest authenticationRequest)
        {
            _logger.LogInformation($"Login attempt from {authenticationRequest.Login}");//tttt@ssss.ru

            AuthenticationResponse authenticationResponse = _authenticateService.Login(authenticationRequest);
            if (authenticationResponse.Status == Models.AuthenticationStatusEnum.Success)
            {
                Response.Headers.Add("X-Session-Token", authenticationResponse.SessionContext.SessionToken);
            }

            _logger.LogInformation($"Login attempt status is {authenticationResponse.Status}");
            return Ok(authenticationResponse);
        }

        [HttpGet("session")]
        public ActionResult<SessionContext> GetSession()
        {
            _logger.LogInformation($"GetSession");

            //[Authorization: Bearer XXXXXXXXXXXXXXXXXXXXXXXX]
            var authorization = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var scheme = headerValue.Scheme; // "Bearer"
                var sessionToken = headerValue.Parameter; // Token

                if (string.IsNullOrEmpty(sessionToken))
                {
                    _logger.LogWarning($"GetSession session Token is null. return Unauthorized");
                    return Unauthorized();
                }


                SessionContext sessionContext = _authenticateService.GetSessionInfo(sessionToken);
                if (sessionContext == null)
                {
                    _logger.LogWarning($"GetSession session context is null. return Unauthorized");
                    return Unauthorized();
                }

                _logger.LogInformation($"GetSession ok for {sessionContext.Account.EMail}");
                return Ok(sessionContext);
            }

            _logger.LogWarning($"GetSession authorization parsing failed. return Unauthorized");
            return Unauthorized();
        }

        #endregion
    }
}
