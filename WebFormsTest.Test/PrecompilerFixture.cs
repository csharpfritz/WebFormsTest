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

  public class PrecompilerFixture : IDisposable
  {

    internal WebApplicationProxy _precompiler;

    public PrecompilerFixture()
    {

      Uri codeBase = new Uri(GetType().Assembly.CodeBase);
      var currentFolder = new DirectoryInfo(Path.GetDirectoryName(codeBase.LocalPath));
      var webFolder = currentFolder.Parent.Parent.Parent.GetDirectories("WebFormsTest.Web")[0];

      _precompiler = new WebApplicationProxy(webFolder.FullName, true);
      _precompiler.Initialize();

    }

    public void Dispose()
    {
      _precompiler.Dispose();
    }

  }

  [Collection("Precompiler collection")]
  public class PrecompilerTests 
  {

    private PrecompilerFixture _Fixture;
    private ITestOutputHelper _testHelper;

    public PrecompilerTests(PrecompilerFixture fixture, ITestOutputHelper helper)
    {
      _Fixture = fixture;

      _testHelper = helper;
      _testHelper.WriteLine("Target folder: " + _Fixture._precompiler.WebApplicationRootFolder);
    }

    [Fact]
    public void GetPageByType()
    {

      // Get the default page
      var t = _Fixture._precompiler.GetPageByLocation("/Scenarios/Postback/Textbox_StaticId.aspx");

      _testHelper.WriteLine("Type returned: " + t.GetType().FullName);

      var a = t.GetType().Assembly;
      _testHelper.WriteLine("ASPNet assembly at: " + a.Location);

      t = _Fixture._precompiler.GetPageByLocation("/Default.aspx");

    }

    [Fact]
    public void BeginProcessing()
    {

      var sut = _Fixture._precompiler.GetPageByLocation("/Default.aspx") as _Default;

      sut.Context = new EmptyHttpContext();

      sut.PrepareTests();

      _testHelper.WriteLine("Controls: " + sut.Controls.Count);
      Assert.NotEqual(0, sut.Controls.Count);
      _testHelper.WriteLine("WE GOT THE CONTROL TREE!!");

    }


    public class EmptyHttpContext : HttpContextBase { }


    [CollectionDefinition("Precompiler collection")]
    public class PrecompiledWebCollection : ICollectionFixture<PrecompilerFixture>
    {
      // This class has no code, and is never created. Its purpose is simply
      // to be the place to apply [CollectionDefinition] and all the
      // ICollectionFixture<> interfaces.
    }


  }
}
