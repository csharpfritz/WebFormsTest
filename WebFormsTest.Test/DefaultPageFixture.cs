using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebFormsTest;
using Xunit;

namespace WebFormsTest.Test
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
    public void BasePageFormHandled()
    {

      // Arrange
      var fakeForm = new NameValueCollection();
      fakeForm.Add("test", "item");
      request.SetupGet(r => r.Form).Returns(fakeForm);

      // Act
      var sut = new WebFormsTest._Default()
      {
        Context = context.Object
      };
      sut.FireEvent(TestablePage.WebFormEvent.Load, new EventArgs());

      // Assert
      response.Verify(r => r.Write("item"), "Did not write the content of the Form");


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

  }

}
