using Fritz.WebFormsTest.Web;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace Fritz.WebFormsTest.Test
{

  public class AutoEventFixture
  {

    private readonly MockRepository _Mockery;
    private readonly Mock<HttpContextBase> context;
    private readonly Mock<HttpResponseBase> response;
    private readonly Mock<HttpRequestBase> request;

    public AutoEventFixture()
    {

      _Mockery = new MockRepository(MockBehavior.Loose);

      context = _Mockery.Create<HttpContextBase>();

      response = _Mockery.Create<HttpResponseBase>();
      context.SetupGet(c => c.Response).Returns(response.Object);
      context.SetupGet(c => c.IsDebuggingEnabled).Returns(true);

      request = _Mockery.Create<HttpRequestBase>();
      context.SetupGet(c => c.Request).Returns(request.Object);

    }

    //[Fact]
    /// <summary>
    /// Return to inspecting this test after the Rehydrate capability is built
    /// </summary>
    public void LoadEventWiredUp()
    {

      // Arrange

      // Act
      var sut = new AutoEventWireup
      {
        Context = context.Object
      };
      sut.FireEvent(WebFormsTest.TestablePage.WebFormEvent.Load, new EventArgs());

      // Assert
      response.Verify(r => r.Write(AutoEventWireup.LOAD_INDICATOR), "Load event was not triggered by AutoEventWireup");

    }

  }

}
