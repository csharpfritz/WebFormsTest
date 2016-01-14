using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Fritz.WebFormsTest
{

  /// <summary>
  /// A class that provides testable implementations of HttpContext objects 
  /// </summary>
  public class TestHttpContext : HttpContextBase
  {

    #region Fields

    private readonly StringBuilder _ResponseStringBuilder;

    #endregion

    public static TestHttpContext Create(Uri requestUri)
    {
      return Create(requestUri.ToString());
    }

    public static TestHttpContext Create(string requestUri)
    {

      var req = CreateDefaultRequest(requestUri);
      var res = CreateDefaultResponse();

      return Create(req, res);

    }

    public static TestHttpContext Create(HttpRequestBase request, HttpResponseBase response)
    {

      return new TestHttpContext()
      {
        Request = request,
        Response = response
      };

    }

    public new HttpRequestBase Request { get; set; }

    public new HttpResponseBase Response { get; set; }

    public override HttpApplicationStateBase Application
    {
      get
      {
        // TODO: Route this to the HttpApplication in WebApplicationProxy
        return base.Application;
      }
    }

    #region Private Methods

    private static HttpRequestBase CreateDefaultRequest(string requestUri)
    {

      // TODO: Create TestHttpRequest class

      throw new NotImplementedException();
    }

    private static HttpResponseBase CreateDefaultResponse()
    {

      // TODO: Create TestHttpResponse class

      throw new NotImplementedException();
    }

    #endregion

  }

}
