using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Trabalho.Startup))]
namespace Trabalho
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
