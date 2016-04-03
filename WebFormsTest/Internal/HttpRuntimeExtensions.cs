using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Fritz.WebFormsTest.Internal
{
  internal static class HttpRuntimeExtensions
  {

    private static readonly Type HttpRuntimeType = typeof(HttpRuntime);

    public static HttpRuntime SetAppDomainVPath(this HttpRuntime runtime, VirtualPathWrapper virtualPath)
    {

      var p = HttpRuntimeType.GetField("_appDomainAppVPath", BindingFlags.NonPublic | BindingFlags.Instance);
      p.SetValue(runtime, virtualPath.VirtualPath);

      return runtime;

    }

    /// <summary>
    /// The the app path of the web application, this should point to the directory the code generator is outputting
    /// </summary>
    /// <param name="runtime"></param>
    /// <param name="codeGenDir"></param>
    /// <returns></returns>
    public static HttpRuntime SetAppDomainAppPath(this HttpRuntime runtime, string codeGenDir)
    {

      var p = HttpRuntimeType.GetField("_appDomainAppPath", BindingFlags.NonPublic | BindingFlags.Instance);
      p.SetValue(runtime, codeGenDir);

      p = typeof(HttpRuntime).GetField("_codegenDir", BindingFlags.NonPublic | BindingFlags.Instance);
      p.SetValue(runtime, codeGenDir);

      return runtime;

    }

    public static HttpRuntime SetAppId(this HttpRuntime runtime, object appId)
    {

      var p = HttpRuntimeType.GetField("_appDomainAppId", BindingFlags.NonPublic | BindingFlags.Instance);
      p.SetValue(runtime, appId);

      return runtime;

    }

    public static HttpRuntime SetPhysicalPath(this HttpRuntime runtime, string physicalPath)
    {

      var p = HttpRuntimeType.GetField("_DefaultPhysicalPathOnMapPathFailure", BindingFlags.NonPublic | BindingFlags.Static);
      p.SetValue(null, physicalPath);

      return runtime;

    }

  }
}
