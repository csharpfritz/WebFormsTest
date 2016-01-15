using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;

namespace Fritz.WebFormsTest.Internal
{

  public class TestVirtualDirectory : VirtualDirectory
  {

    private string _VirtualPath;
    private string _PhysicalPath;
    private DirectoryInfo _DirectoryInfo;

    public TestVirtualDirectory(string virtualPath) : base(virtualPath)
    {
      _VirtualPath = virtualPath;
      _PhysicalPath = Path.Combine(WebApplicationProxy.WebRootFolder, virtualPath.Substring(1));
      _DirectoryInfo = new DirectoryInfo(_PhysicalPath);
    }

    public override IEnumerable Children
    {
      get
      {
        var outChildren = new List<VirtualFileBase>();
        outChildren.AddRange(Directories as List<TestVirtualDirectory>);
        outChildren.AddRange(Files as List<TestVirtualFile>);
        return outChildren;
      }
    }

    public override IEnumerable Directories
    {
      get
      {

        var dirs = _DirectoryInfo.GetDirectories().ToList();
        var outDirs = new List<TestVirtualDirectory>();

        var workingVirtualPath = _VirtualPath.EndsWith("/") ? _VirtualPath.Substring(0, _VirtualPath.Length - 1) : _VirtualPath;


        foreach (var d in dirs)
        {
          outDirs.Add(new TestVirtualDirectory(workingVirtualPath + "/" + d.Name));
        }

        return outDirs;
      }
    }

    public override IEnumerable Files
    {
      get
      {

        var files = _DirectoryInfo.GetFiles();
        var outFiles = new List<TestVirtualFile>();
        var workingVirtualPath = _VirtualPath.EndsWith("/") ? _VirtualPath.Substring(0, _VirtualPath.Length - 1) : _VirtualPath;

        foreach (var f in files)
        {
          outFiles.Add(new TestVirtualFile(workingVirtualPath + f.Name));
        }

        return outFiles;

      }
    }
  }

}
