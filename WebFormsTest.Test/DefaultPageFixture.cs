using Fritz.WebFormsTest.Web;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using Xunit;

namespace Fritz.WebFormsTest.Test
{

  public class DefaultPageFixture
  {

    private readonly MockRepository _Mockery;
    private readonly Mock<HttpContextBase> context;
    private readonly Mock<HttpResponseBase> response;
    private readonly Mock<HttpRequestBase> request;

    public DefaultPageFixture()
    {

      _Mockery = new MockRepository(MockBehavior.Loose);

      context = _Mockery.Create<HttpContextBase>();
      
      response = _Mockery.Create<HttpResponseBase>();
      context.SetupGet(c => c.Response).Returns(response.Object);
      context.SetupGet(c => c.IsDebuggingEnabled).Returns(true);

      request = _Mockery.Create<HttpRequestBase>();
      context.SetupGet(c => c.Request).Returns(request.Object);

    }

    [Fact]
    public void VerifyIsInTestModeIsSetInHttpContextCurrent()
    {

      // Arrange
      WebApplicationProxy.SubstituteDummyHttpContext("/");

      // Act
      HttpContext testContext = HttpContext.Current;

      // Assert
      Assert.NotNull(testContext);
      Assert.True(testContext.Items.Contains("IsInTestMode"));

    }

  }

}
