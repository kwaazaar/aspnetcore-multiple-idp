using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace web.auth.BasicAuth
{
    // Based on: https://github.com/blowdart/idunno.Authentication/tree/dev/src/idunno.Authentication.Basic
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        public const string DefaultScheme = "Basic";

        public BasicAuthenticationHandler(
            IOptionsMonitor<BasicAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        /// <summary>
        /// Creates a new instance of the events instance.
        /// </summary>
        /// <returns>A new instance of the events instance.</returns>
        //protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new BasicAuthenticationEvents());

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            await Task.CompletedTask; // To prevent return Task.FromResult everywhere

            string authorizationHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader)) // No header at all
            {
                return AuthenticateResult.NoResult();
            }

            if (!authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase)) // No basic auth header
            {
                return AuthenticateResult.NoResult();
            }

            string encodedCredentials = authorizationHeader.Substring(authorizationHeader.IndexOf(' ') + 1).Trim();
            if (string.IsNullOrEmpty(encodedCredentials))
            {
                const string noCredentialsMessage = "No credentials";
                Logger.LogInformation(noCredentialsMessage);
                return AuthenticateResult.Fail(noCredentialsMessage);
            }

            try
            {
                string username;
                string password;
                try
                {
                    var base64DecodedCredentials = Convert.FromBase64String(encodedCredentials);
                    var decodedCredentials = Encoding.UTF8.GetString(base64DecodedCredentials);
                    var parts = decodedCredentials.Split(':');
                    username = parts[0];
                    password = parts.Length > 1 ? string.Join(':', parts.Skip(1)) : null;
                }
                catch (Exception)
                {
                    const string failedToParseCredentials = "Cannot parse credentials";
                    Logger.LogInformation(failedToParseCredentials);
                    return AuthenticateResult.Fail(failedToParseCredentials);
                }

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    const string usernameOrPasswordEmpty = "Invalid credentials, username or password empty";
                    Logger.LogInformation(usernameOrPasswordEmpty);
                    return AuthenticateResult.Fail(usernameOrPasswordEmpty);
                }

                if (username != password) // ;-D
                {
                    return AuthenticateResult.Fail("invalid credentials");
                }

                var claimSet = new List<Claim>(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username),
                });

                var principal = new ClaimsPrincipal(new ClaimsIdentity(claimSet, "Basic"));
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "error validating basic auth credentials");
                return AuthenticateResult.Fail("technical issue");
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;
            Response.Headers.Append(HeaderNames.WWWAuthenticate, $"Basic realm=\"{Options.ClaimsIssuer}\"");

            return Task.CompletedTask;
        }

        //var endpoint = Context.GetEndpoint();
        //if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        //    return AuthenticateResult.NoResult();

        //if (!Request.Headers.ContainsKey("Authorization"))
        //    return AuthenticateResult.Fail("Missing Authorization Header");

        //User user = null;
        //try
        //{
        //    var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
        //    var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
        //    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
        //    var username = credentials[0];
        //    var password = credentials[1];
        //    user = await _userService.Authenticate(username, password);
        //}
        //catch
        //{
        //    return AuthenticateResult.Fail("Invalid Authorization Header");
        //}

        //if (user == null)
        //    return AuthenticateResult.Fail("Invalid Username or Password");

        //var claims = new[] {
        //    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //    new Claim(ClaimTypes.Name, user.Username),
        //};
        //var identity = new ClaimsIdentity(claims, Scheme.Name);
        //var principal = new ClaimsPrincipal(identity);
        //var ticket = new AuthenticationTicket(principal, Scheme.Name);

        //return AuthenticateResult.Success(ticket);
    
    }
}
