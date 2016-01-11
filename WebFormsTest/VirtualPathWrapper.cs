using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;

namespace Fritz.WebFormsTest
{

  /// <summary>
  /// A wrapper class that gives access to the System.Web internal VirtualPath object
  /// </summary>
  internal class VirtualPathWrapper
  {

    public static readonly Type VirtualPathType;

    /// <summary>
    /// The VirtualPath object that we are managing requests to
    /// </summary>
    public object VirtualPath { get; private set; }

    static VirtualPathWrapper()
    {

      // Capture the Type of VirtualPath
      var a = typeof(System.Web.UI.Page).Assembly;
      VirtualPathType = a.GetType("System.Web.VirtualPath");

    }

    public static VirtualPathWrapper Create(string path)
    {

      var mi = VirtualPathType.GetMethod("Create", new[] { typeof(string) });

      var outObject = new VirtualPathWrapper()
      {
        VirtualPath = mi.Invoke(null, new object[] { path })
      };
      return outObject;


    }

    public static VirtualPathWrapper CreateAbsolute(string path)
    {

      var mi = VirtualPathType.GetMethod("CreateAbsolute", new[] { typeof(string) });

      return new VirtualPathWrapper()
      {
        VirtualPath = mi.Invoke(null, new object[] { path })
      };

    }

    public static object CreateNonRelativeTrailingSlash(string path)
    {

      var mi = VirtualPathType.GetMethod("CreateNonRelativeTrailingSlash", new[] { typeof(string) });

      return mi.Invoke(null, new object[] { path });
    }

    public static object SimpleCombine(string newPath)
    {

      var rootPath = Create("/");

      var mi = VirtualPathType.GetMethod("SimpleCombine", BindingFlags.NonPublic | BindingFlags.Instance);

      return mi.Invoke(rootPath, new object[] { newPath, true });

    }

    /// <summary>
    /// Provide access to the VirtualPathString property of the VirtualPath object
    /// </summary>
    public string VirtualPathString
    {
      get
      {

        var pi = VirtualPathType.GetProperty("VirtualPathString", BindingFlags.Instance | BindingFlags.Public);
        Debug.Assert(pi != null, "Property not found");
        return pi.GetValue(VirtualPath, null).ToString();

      }
    }

    //private static string MapPath(object virtualPath, object baseVirtualPath)
    //{

    //  var vp = Combine(virtualPath, baseVirtualPath);

    //  var mi = VirtualPathType.GetMethod("MapPathInternal", BindingFlags.NonPublic | BindingFlags.Instance);
    //  return mi.Invoke(vp, null).ToString();

    //}

    //public static string MapPath(string virtualPath)
    //{

    //  var myVPath = Create(virtualPath);

    //  return VirtualPathWrapper. MapPath(WebApplicationProxy.BaseVirtualPath, myVPath);
      
    //}

  }

}
