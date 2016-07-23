using Fritz.WebFormsTest.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xunit;
using Xunit.Abstractions;

namespace Fritz.WebFormsTest.Test
{

  public class PrecompiledWebConfiguration : IDisposable
  {

    public PrecompiledWebConfiguration()
    {

      Uri codeBase = new Uri(GetType().Assembly.CodeBase);
      var currentFolder = new DirectoryInfo(Path.GetDirectoryName(codeBase.LocalPath));
      WebFolder = currentFolder.Parent.Parent.Parent.GetDirectories("WebFormsTest.Web")[0];

      //WebApplicationProxy.Create(WebFolder.FullName, true);
      WebApplicationProxy.Create(typeof(_Default));

    }

    public DirectoryInfo WebFolder { get; private set; }

    public void Dispose()
    {
      WebApplicationProxy.DisposeIt();
    }

  }

  [CollectionDefinition("Precompiler collection")]
  public class PrecompiledWebCollection : ICollectionFixture<PrecompiledWebConfiguration>
  {
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
  }


}
