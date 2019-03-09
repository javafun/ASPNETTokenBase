using System.Security.Claims;
using System.Threading.Tasks;
using AngularJSAuthentication.API.Repositories;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.OAuth;

namespace AngularJSAuthentication.API.Providers
{
    internal class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        // This is responsible for validating the "Client", in this case we assume we only have one client.
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult(0);
        }

        // This is responsible to validate the username and password sent to the authorization server’s token endpoint.
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // To allow CORS on the token middleware provider we need to add the header “Access-Control-Allow-Origin” 
            // to Owin context, if you forget this, generating the token will fail when you try to call it from 
            // your browser. 
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            using (var repo = new AuthRepository())
            {
                IdentityUser user = await repo.FindUser(context.UserName, context.Password);

                if(user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
            }
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role","user"));

            context.Validated(identity);
        }
    }
}