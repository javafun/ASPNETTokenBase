using Microsoft.Owin;
using Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(AngularJSAuthentication.API.Startup))]
namespace AngularJSAuthentication.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            // The "HttpConfiguration" object is used to configure API routes, 
            // so we’ll pass this object to method "Register" in "WebApiConfig" class.
            WebApiConfig.Register(config);

            // "UseWebApi" will be responsible to wire up ASP.NET Web API to our Owin server pipeline.
            app.UseWebApi(config);
        }
    }
}
