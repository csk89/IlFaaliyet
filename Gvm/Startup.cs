using Gvm.Infra;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Gvm.Startup))]
namespace Gvm
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

           // app..UseResourceAuthorization(new AuthorizationManager());
        }
    }
}
