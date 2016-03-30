using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;

namespace Fritz.WebFormsTest.Internal
{

  public class TestVirtualFile : VirtualFile
  {
    private FileInfo _FileInfo;
    private string _PhysicalPath;
    private string _VirtualPath;

    public TestVirtualFile(string virtualPath) : base(virtualPath)
    {
      _VirtualPath = virtualPath;
      _PhysicalPath = Path.Combine(TestVirtualPathProvider.RootFolder, TestVirtualPathProvider.ReformatPath(virtualPath));
      _FileInfo = new FileInfo(_PhysicalPath);
    }

    public override Stream Open()
    {
      return _FileInfo.Open(FileMode.Open);
    }

    public override string Name
    {
      get
      {
        return _FileInfo.Name;
      }
    }

  }
}
