using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace Fritz.WebFormsTest.Internal
{

  public class TestVirtualPathProvider : VirtualPathProvider
  {

    public override string CombineVirtualPaths(string basePath, string relativePath)
    {
      var outPath = base.CombineVirtualPaths(basePath, relativePath);
      return outPath;
    }

    public override VirtualDirectory GetDirectory(string virtualDir)
    {
      var outDir = new TestVirtualDirectory(virtualDir);
      return outDir;
    }

    public override bool DirectoryExists(string virtualDir)
    {
      var realFolder = virtualDir.Replace("~/", "").Replace('/','\\');
      var physicalFolder = Path.Combine(RootFolder, realFolder);
      return new DirectoryInfo(physicalFolder).Exists;
    }

    public override bool FileExists(string virtualPath)
    {
      string thisPath = ReformatPath(virtualPath);

      var physicalFile = Path.Combine(RootFolder, thisPath);

      var outValue = new FileInfo(physicalFile).Exists;

      return outValue;

    }

    internal static string ReformatPath(string virtualPath)
    {
      var thisPath = virtualPath.Replace("~/", "/");
      if (thisPath.StartsWith("/")) thisPath = thisPath.Substring(1);

      if (thisPath.Contains('/')) thisPath = thisPath.Replace('/', '\\');
      return thisPath;
    }

    public override string GetCacheKey(string virtualPath)
    {
      var outKey = base.GetCacheKey(virtualPath);
      return outKey;
    }

    public override VirtualFile GetFile(string virtualPath)
    {
      return new TestVirtualFile(virtualPath);
    }

    internal static string RootFolder
    {
      get
      {

        var p = typeof(HttpRuntime).GetField("_DefaultPhysicalPathOnMapPathFailure", BindingFlags.NonPublic | BindingFlags.Static);
        return p.GetValue(null).ToString();


      }
    }


  }

}
