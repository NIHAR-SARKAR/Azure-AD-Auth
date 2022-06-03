using System;
using System.Threading;
using Microsoft.Owin;
using Owin;
using Microsoft.Identity.Web;
using Microsoft.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;

[assembly: OwinStartup(typeof(AzureAdWebX.Startup))]

namespace AzureAdWebX
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            Config = config;
        }
        public Startup()
        {

        }
        public IConfiguration Config { get;}
        
        string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"];

       string redirectUri = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];

        static string tenant = System.Configuration.ConfigurationManager.AppSettings["Tenant"];

        string authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, System.Configuration.ConfigurationManager.AppSettings["Authority"], tenant);

        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseOpenIdConnectAuthentication(
            new OpenIdConnectAuthenticationOptions
            {
                // Sets the ClientId, authority, RedirectUri as obtained from web.config
                ClientId = clientId,
                Authority = authority,
                RedirectUri = redirectUri,
                // PostLogoutRedirectUri is the page that users will be redirected to after sign-out. In this case, it is using the home page
                PostLogoutRedirectUri = redirectUri,
                Scope = OpenIdConnectScope.OpenIdProfile,
                // ResponseType is set to request the code id_token - which contains basic information about the signed-in user
                ResponseType = OpenIdConnectResponseType.CodeIdToken,
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = OnAuthenticationFailed
                }
            }
            );
            app.Use(async (context,next) =>
            {
                if (!context.Authentication.User.Identity?.IsAuthenticated ?? false)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Not Authenticated");
                }
                else
                {
                    await next();
                }
            });
        }
        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            return Task.FromResult(0);
        }

    }
}
