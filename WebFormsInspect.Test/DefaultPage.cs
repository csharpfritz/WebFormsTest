using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebFormsInspect.Core;
using Xunit;

namespace WebFormsInspect.Test
{

  public class DefaultPage
  {

    private readonly MockRepository _Mockery;

    public DefaultPage()
    {

      _Mockery = new MockRepository(MockBehavior.Loose);

    }

    [Fact]
    public void BasePageIndicatesResponse()
    {

      // Arrange
      var ctx = _Mockery.Create<HttpContextBase>();
      var response = _Mockery.Create<HttpResponseBase>();
      ctx.SetupGet(c => c.Response).Returns(response.Object);


      // Act
      var sut = new WebFormsInspect._Default()
      {
        Context = ctx.Object
      };
      sut.FireEvent(Core.TestablePage.WebFormEvent.PreRender, new EventArgs());
      var results = sut.RenderHtml();

      // Assert
      Assert.Contains(TestablePage.COMMENT_MARKER, results);

    }

    [Fact]
    public void VerifyOnLoadWasCalled()
    {

      // Arrange
      var ctx = _Mockery.Create<HttpContextBase>();
      var response = _Mockery.Create<HttpResponseBase>();
      ctx.SetupGet(c => c.Response).Returns(response.Object);

      // Act
      var sut = new WebFormsInspect._Default
      {
        Context = ctx.Object
      };
      sut.FireEvent(Core.TestablePage.WebFormEvent.Load, new EventArgs());

      // Assert
      response.Verify(r => r.Write(_Default.LOAD_INDICATOR));

    }

  }

}
