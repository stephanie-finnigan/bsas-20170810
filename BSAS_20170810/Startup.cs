using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BSAS_20170810.Startup))]
namespace BSAS_20170810
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            
        }
    }
}
