using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using static ClinicServiceProtos.AuthenticateService;
using AuthenticationRequest = ClinicServiceProtos.AuthenticationRequest;
using AuthenticationResponse = ClinicServiceProtos.AuthenticationResponse;
//using ClinicServiceProtos; // убрал чтобы было виднее что куда ссылается

namespace ClinicService.Services.Implementation
{
    [Authorize]
    public class AuthService : AuthenticateServiceBase
    {
        #region Services

        private readonly IAuthenticateService _authenticateService;

        #endregion

        #region Constructors

        public AuthService(IAuthenticateService authenticateService)
        {
            _authenticateService = authenticateService;
        }

        #endregion

        [AllowAnonymous]
        public override Task<ClinicServiceProtos.AuthenticationResponse> Login(ClinicServiceProtos.AuthenticationRequest request, ServerCallContext context)
        {
            Models.Responses.AuthenticationResponse response = _authenticateService.Login(new Models.Requests.AuthenticationRequest
            {
                Login = request.UserName,
                Password = request.Password
            });

            if (response.Status == Models.AuthenticationStatusEnum.Success)
            {
                context.ResponseTrailers.Add("X-Session-Token", response.SessionContext.SessionToken);
            }

            return Task.FromResult(new ClinicServiceProtos.AuthenticationResponse
            {
                Status = (int)response.Status,
                SessionContext = new ClinicServiceProtos.SessionContext
                {
                    SessionId = response.SessionContext.SessionId,
                    SessionToken = response.SessionContext.SessionToken,
                    Account = new ClinicServiceProtos.AccountDto
                    {
                        AccountId = response.SessionContext.Account.AccountId,
                        EMail = response.SessionContext.Account.EMail,
                        FirstName = response.SessionContext.Account.FirstName,
                        LastName = response.SessionContext.Account.LastName,
                        SecondName = response.SessionContext.Account.SecondName,
                        Locked = response.SessionContext.Account.Locked
                    }
                }
            });
        }

        public override Task<ClinicServiceProtos.GetSessionResponse> GetSession(ClinicServiceProtos.GetSessionRequest request, ServerCallContext context)
        {
            var authorizationHeader = context.RequestHeaders.FirstOrDefault(header => header.Key == "Authorization");
            if (AuthenticationHeaderValue.TryParse(authorizationHeader.Value, out var headerValue))
            {
                var scheme = headerValue.Scheme; // "Bearer"
                var sessionToken = headerValue.Parameter; // Token
                if (string.IsNullOrEmpty(sessionToken))
                {
                    return Task.FromResult(new ClinicServiceProtos.GetSessionResponse());
                }

                ClinicService.Models.SessionContext sessionContext = _authenticateService.GetSessionInfo(sessionToken);
                if (sessionContext == null)
                {
                    return Task.FromResult(new ClinicServiceProtos.GetSessionResponse());
                }

                return Task.FromResult(new ClinicServiceProtos.GetSessionResponse
                {
                    SessionContext = new ClinicServiceProtos.SessionContext
                    {
                        SessionId = sessionContext.SessionId,
                        SessionToken = sessionContext.SessionToken,
                        Account = new ClinicServiceProtos.AccountDto
                        {
                            AccountId = sessionContext.Account.AccountId,
                            EMail = sessionContext.Account.EMail,
                            FirstName = sessionContext.Account.FirstName,
                            LastName = sessionContext.Account.LastName,
                            SecondName = sessionContext.Account.SecondName,
                            Locked = sessionContext.Account.Locked
                        }
                    }
                });
            }

            return Task.FromResult(new ClinicServiceProtos.GetSessionResponse());
        }
    }
}
