using Fritz.WebFormsTest;
using Fritz.WebFormsTest.Test;
using Fritz.WebFormsTest.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Fritz.WebFormsInspect.Test
{
  [Collection("Precompiler collection")]
  public class WebApplicationProxyFixture
  {

    private PrecompilerFixture _Fixture;
    private ITestOutputHelper _testHelper;

    public WebApplicationProxyFixture(PrecompilerFixture fixture, ITestOutputHelper helper)
    {
      _Fixture = fixture;

      _testHelper = helper;
      _testHelper.WriteLine("Target folder: " + WebApplicationProxy.WebApplicationRootFolder);
    }

    [Fact]
    public void GetPageByType()
    {

      // Get the default page
      var t = WebApplicationProxy.GetPageByLocation("/Scenarios/Postback/Textbox_StaticId.aspx");

      _testHelper.WriteLine("Type returned: " + t.GetType().FullName);

      var a = t.GetType().Assembly;
      _testHelper.WriteLine("ASPNet assembly at: " + a.Location);

      t = WebApplicationProxy.GetPageByLocation("/Default.aspx");

    }

    [Fact]
    public void BeginProcessing()
    {

      var sut = WebApplicationProxy.GetPageByLocation("/Default.aspx") as _Default;

      sut.Context = new TestablePage.EmptyHttpContext();

      sut.PrepareTests();

      _testHelper.WriteLine("Controls: " + sut.Controls.Count);
      Assert.NotEqual(0, sut.Controls.Count);
      _testHelper.WriteLine("WE GOT THE CONTROL TREE!!");

    }

  }

}
