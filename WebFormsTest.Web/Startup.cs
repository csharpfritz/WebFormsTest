using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Fritz.WebFormsTest.Web.Startup))]
namespace Fritz.WebFormsTest.Web
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            //ConfigureAuth(app);
        }
    }
}
