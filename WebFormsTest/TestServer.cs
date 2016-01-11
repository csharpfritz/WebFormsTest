using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Fritz.WebFormsTest
{

  /// <summary>
  /// An implementation of <see cref="HttpServerUtilityBase"/> that provides for testing
  /// </summary>
  public class TestServer : HttpServerUtilityBase
  {

    /// <summary>
    /// According to referencesource.microsoft.com, this is routed through Request.MapPath, which we expect most testers will by mocking or replacing.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public override string MapPath(string path)
    {

      if (!WebApplicationProxy.IsInitialized) throw new InvalidOperationException("WebApplicationProxy needs to be initialized before using MapPath");

      // NOTE: YAGNI -- make this a simple implementation that concatenates the two paths

      var vpath = VirtualPathWrapper.Create(path);

      // Drop initial slash before we join paths
      var virtualPathString = vpath.VirtualPathString.Substring(1);

      return System.IO.Path.Combine(WebApplicationProxy.WebRootFolder, virtualPathString);

    }

    public override int ScriptTimeout { get; set; }

  }

}
