using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Fritz.WebFormsTest.Internal
{

  /// <summary>
  /// An internal resource that manages the application inside of the ShadowDomain
  /// </summary>
  internal class WebApplicationManager : MarshalByRefObject
  {

    private WebApplicationProxyOptions _Options;
    private bool _Initialized;

    public WebApplicationManager() { }

    public void Initialize(WebApplicationProxyOptions options)
    {

      _Options = options;

      AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", Path.Combine(options.PhysicalRootFolder, "web.config"));

      _Initialized = true;

    }

    public bool IsInitialized() { return _Initialized; }

    public object GetByLocation(string location, HttpBrowserCapabilities browserCaps, Action<HttpContext> contextModifiers = null)
    {

      return null;

    }

    private static HttpRuntime HttpRuntimeInstance
    {
      get
      {
        var getRunTime = typeof(HttpRuntime).GetField("_theRuntime", BindingFlags.NonPublic | BindingFlags.Static);
        return getRunTime.GetValue(null) as HttpRuntime;
      }
    }

  }

}
