using Fritz.WebFormsTest.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace Fritz.WebFormsTest.Test
{

  [Collection("Precompiler collection")]
  public class SessionFixture
  {

    [Fact]
    public void SessionHasSessionId()
    {

      // Arrange

      // Act
      WebApplicationProxy.GetPageByLocation("/default.aspx");
      var sut = WebApplicationProxy.GetNewSession();

      // Assert
      Assert.NotNull(HttpContext.Current.Session);
      Assert.False(string.IsNullOrEmpty(HttpContext.Current.Session.SessionID));

    }

  }

}
