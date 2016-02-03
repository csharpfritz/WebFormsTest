using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
      var physicalFolder = Path.Combine(WebApplicationProxy.WebPrecompiledFolder, realFolder);
      return new DirectoryInfo(physicalFolder).Exists;
    }

    public override bool FileExists(string virtualPath)
    {

      virtualPath = virtualPath.Replace("~/", "/").Substring(1).Replace('/','\\');
      var physicalFile = Path.Combine(WebApplicationProxy.WebRootFolder, virtualPath);

      return new FileInfo(physicalFile).Exists;
    }

    public override string GetCacheKey(string virtualPath)
    {
      var outKey = base.GetCacheKey(virtualPath);
      return outKey;
    }

  }

}
