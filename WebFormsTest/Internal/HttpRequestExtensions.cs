using System.Reflection;
using System.Web;

namespace Fritz.WebFormsTest.Internal
{

  internal static class HttpRequestExtensions
  {

    /// <summary>
    /// Define the requesting browser's capabilities
    /// </summary>
    /// <param name="request"></param>
    /// <param name="browserCaps"></param>
    public static void SetBrowserCaps(this HttpRequest request, HttpBrowserCapabilities browserCaps) {

      var theField = typeof(HttpRequest).GetField("_browsercaps", BindingFlags.Instance | BindingFlags.NonPublic);
      theField.SetValue(request, browserCaps);

    }

  }

}
