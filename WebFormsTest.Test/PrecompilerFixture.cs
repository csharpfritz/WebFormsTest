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

    internal WebFormsPrecompiler _precompiler;

    public PrecompilerFixture()
    {

      Uri codeBase = new Uri(GetType().Assembly.CodeBase);
      var currentFolder = new DirectoryInfo(Path.GetDirectoryName(codeBase.LocalPath));
      var webFolder = currentFolder.Parent.Parent.Parent.GetDirectories("WebFormsTest.Web")[0];

      _precompiler = new WebFormsTest.WebFormsPrecompiler(webFolder.FullName, typeof(_Default).Assembly);
    }

    public void Dispose()
    {
      _precompiler.Dispose();
    }

  }

  public class PrecompilerTests : IClassFixture<PrecompilerFixture>
  {

    private PrecompilerFixture _Fixture;
    private ITestOutputHelper _testHelper;

    public PrecompilerTests(PrecompilerFixture fixture, ITestOutputHelper helper) //       PrecompilerFixture fixture, 
    {
      _Fixture = fixture;

      _testHelper = helper;
      _testHelper.WriteLine("Target folder: " + _Fixture._precompiler.TargetFolder);
    }

    [Fact]
    public void GetPageByType()
    {

      // Get the default page
      var t = _Fixture._precompiler.CompilePage("/Scenarios/Postback/Textbox_StaticId.aspx");

      _testHelper.WriteLine("Type returned: " + t.FullName);

      var a = Assembly.GetAssembly(t);
      _testHelper.WriteLine("ASPNet assembly at: " + a.Location);

      var sut = Activator.CreateInstance(t);
      _testHelper.WriteLine("New page: " + sut.GetType());

      t = _Fixture._precompiler.CompilePage("/Default.aspx");

      _testHelper.WriteLine("Type returned: " + t.FullName);

      a = Assembly.GetAssembly(t);
      _testHelper.WriteLine("ASPNet assembly at: " + a.Location);

      sut = Activator.CreateInstance(t);
      _testHelper.WriteLine("New page: " + sut.GetType());

    }

    [Fact]
    public void BeginProcessing()
    {

      var t = _Fixture._precompiler.CompilePage("/Default.aspx");

      _testHelper.WriteLine("Type returned: " + t.FullName);

      var sut = Activator.CreateInstance(t) as _Default;
      _testHelper.WriteLine("New page: " + sut.GetType());

      sut.Context = new EmptyHttpContext();

      sut.PrepareToProcess(sut);

      _testHelper.WriteLine("Controls: " + sut.Controls.Count);
      Assert.NotEqual(0, sut.Controls.Count);
      _testHelper.WriteLine("WE GOT THE CONTROL TREE!!");

    }

    public class EmptyHttpContext : HttpContextBase { }

  }
}
