using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebFormsTest;
using Xunit;

namespace WebFormsTest.Test
{

  public class DefaultPage
  {

    private readonly MockRepository _Mockery;
    private readonly Mock<HttpContextBase> context;
    private readonly Mock<HttpResponseBase> response;

    public DefaultPage()
    {

      _Mockery = new MockRepository(MockBehavior.Loose);

      context = _Mockery.Create<HttpContextBase>();
      response = _Mockery.Create<HttpResponseBase>();
      context.SetupGet(c => c.Response).Returns(response.Object);
      context.SetupGet(c => c.IsDebuggingEnabled).Returns(true);


    }

    [Fact]
    public void BasePageIndicatesResponse()
    {

      // Arrange


      // Act
      var sut = new WebFormsTest._Default()
      {
        Context = context.Object
      };
      sut.FireEvent(TestablePage.WebFormEvent.PreRender, new EventArgs());
      var results = sut.RenderHtml();

      // Assert
      Assert.Contains(TestablePage.COMMENT_MARKER, results);

    }

    [Fact]
    public void VerifyOnLoadWasCalled()
    {

      // Arrange 

      // Act
      var sut = new WebFormsTest._Default
      {
        Context = context.Object
      };
      sut.FireEvent(TestablePage.WebFormEvent.Load, new EventArgs());

      // Assert
      response.Verify(r => r.Write(_Default.LOAD_INDICATOR));

    }

    [Fact]
    public void TestPostback()
    {

      // Arrange

      // Act
      var sut = new _Default();
      sut.MockPostData(null);

      // Assert

    }

  }

}
