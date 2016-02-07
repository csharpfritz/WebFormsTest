using Fritz.WebFormsTest.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
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
      var sut = WebApplicationProxy.GetNewSession();

      // Assert
      Assert.False(string.IsNullOrEmpty(sut.SessionID));

    }

    [Fact]
    public void SessionOnPageMatchesHttpContextCurrent()
    {

      // Arrange

      // Act
      var sut = WebApplicationProxy.GetNewSession();
      var thePage = WebApplicationProxy.GetPageByLocation("/default.aspx", ctx => ctx.AddSession(sut)) as Page;

      // Assert
      Assert.Same(sut, thePage.Session);
      Assert.NotNull(HttpContext.Current);

    }

  }

}
