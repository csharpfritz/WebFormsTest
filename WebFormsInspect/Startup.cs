using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebFormsInspect.Startup))]
namespace WebFormsInspect
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
