using Fritz.WebFormsTest.Web;
using System;
using System.Collections.Generic;
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

    private WebFormsPrecompiler _precompiler;
    private ITestOutputHelper _testHelper;

    public PrecompilerFixture(ITestOutputHelper helper)
    {
      _testHelper = helper;
      _precompiler = new WebFormsTest.WebFormsPrecompiler("c:\\dev\\WebFormsTest\\WebFormsTest.Web", typeof(_Default).Assembly);
      _testHelper.WriteLine("Target folder: " + _precompiler.TargetFolder);
    }

    public void Dispose()
    {
      _precompiler.Dispose();
    }

    [Fact]
    public void GetPageByType()
    {

      // Get the default page
      var t = _precompiler.CompilePage("/Scenarios/Postback/Textbox_StaticId.aspx");

      _testHelper.WriteLine("Type returned: " + t.FullName);

      var a = Assembly.GetAssembly(t);
      _testHelper.WriteLine("ASPNet assembly at: " + a.Location);

      var sut = Activator.CreateInstance(t);
      _testHelper.WriteLine("New page: " + sut.GetType());

      t = _precompiler.CompilePage("/Default.aspx");

      _testHelper.WriteLine("Type returned: " + t.FullName);

      a = Assembly.GetAssembly(t);
      _testHelper.WriteLine("ASPNet assembly at: " + a.Location);

      sut = Activator.CreateInstance(t);
      _testHelper.WriteLine("New page: " + sut.GetType());

    }

    [Fact]
    public void BeginProcessing()
    {

      var t = _precompiler.CompilePage("/Default.aspx");

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
