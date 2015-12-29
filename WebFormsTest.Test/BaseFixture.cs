using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Fritz.WebFormsTest.Test
{

  public abstract class BaseFixture
  {

    protected readonly MockRepository _Mockery;
    protected readonly Mock<HttpContextBase> context;
    protected readonly Mock<HttpResponseBase> response;
    protected readonly Mock<HttpRequestBase> request;

    public BaseFixture()
    {

      _Mockery = new MockRepository(MockBehavior.Loose);

      context = _Mockery.Create<HttpContextBase>();

      response = _Mockery.Create<HttpResponseBase>();
      context.SetupGet(c => c.Response).Returns(response.Object);
      context.SetupGet(c => c.IsDebuggingEnabled).Returns(true);

      request = _Mockery.Create<HttpRequestBase>();
      context.SetupGet(c => c.Request).Returns(request.Object);

    }


  }

}
