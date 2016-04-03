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

  }
}
