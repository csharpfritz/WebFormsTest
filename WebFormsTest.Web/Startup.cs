using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebFormsTest.Startup))]
namespace WebFormsTest
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
