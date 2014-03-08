using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LobbyServer.Startup))]
namespace LobbyServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
