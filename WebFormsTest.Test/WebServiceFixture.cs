using Fritz.WebFormsTest.Web.Scenarios.WebServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace Fritz.WebFormsTest.Test
{

  /// <summary>
  /// Verift that web services are able to be constructed properly
  /// </summary>
  [Collection("Precompiler collection")]
  public class WebServiceFixture
  {
    
    [Fact]
    public void CheckContextIsInjectedIntoWebService()
    {

      var service = WebApplicationProxy.GetService<TestWebService>();

      Assert.Equal(HttpContext.Current, service.Context);
    }

    [Fact]
    public void CheckSessionExistsOnInjectedContextWithSessionOnWebService()
    {

      var session = WebApplicationProxy.GetNewSession(new Dictionary<string, object>
      {
        ["TestCode"] = "123"
      });

      var service = WebApplicationProxy.GetService<TestWebService>(context =>
      {
        context.AddSession(session);
      });

      Assert.Equal("123", service.Session["TestCode"]);
    }


  }
}
