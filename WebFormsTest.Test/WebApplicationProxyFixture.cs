using Fritz.WebFormsTest;
using Fritz.WebFormsTest.Test;
using Fritz.WebFormsTest.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Xunit;
using Xunit.Abstractions;

namespace Fritz.WebFormsTest.Test
{
  [Collection("Precompiler collection")]
  public class WebApplicationProxyFixture : BaseFixture
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


      _testHelper.WriteLine("Controls: " + sut.Controls.Count);
      Assert.NotEqual(0, sut.Controls.Count);
      _testHelper.WriteLine("WE GOT THE CONTROL TREE!!");

    }

    [Fact]
    public void CheckRouteTableIsPopulated()
    {

      // Arrange

      // Act
      var sut = RouteTable.Routes;

      // Assert
      Assert.NotEqual(0, sut.Count);

    }

    [Fact]
    public void GetTheHttpApplicationTypeFindsGlobal()
    {

      // Arrange

      // Act
      var t = WebApplicationProxy.GetHttpApplicationType();

      // Assert
      Assert.NotNull(t);

    }

    /// <summary>
    /// This doesn't feel right here, and feels like we need a ReydratedTestablePageFixture
    /// </summary>
    [Fact]
    public void ServerPropertyHandlesMapPath()
    {

      // Arrange

      // Act
      var defaultPage = WebApplicationProxy.GetPageByLocation<_Default>("/Default.aspx");
      var sut = defaultPage.Server;
      var baseFolder = sut.MapPath("/");

      // Assert
      _testHelper.WriteLine("Web Folder Name: " + _Fixture.WebFolder.FullName);
      Assert.Equal(_Fixture.WebFolder.FullName, baseFolder);

    }

    [Fact]
    public void ServerMapPathHandlesAppRelativePath()
    {

      // Arrange

      // Act
      var defaultPage = WebApplicationProxy.GetPageByLocation<_Default>("/Default.aspx");
      var sut = defaultPage.Server;
      var resultFolder = sut.MapPath("~/Scenarios");

      // Assert
      _testHelper.WriteLine("Web Folder Name: " + _Fixture.WebFolder.FullName);
      Assert.Equal(System.IO.Path.Combine(_Fixture.WebFolder.FullName, "Scenarios"), resultFolder);

    }

    [Fact]
    public void GetPageByLocationShouldHandleContext()
    {

      // Arrange
      var myDict = new Dictionary<string,string>();
      myDict.Add("test", "test");
      base.context.SetupGet(c => c.Items).Returns(myDict);

      // Act
      var defaultPage = WebApplicationProxy.GetPageByLocation<_Default>("/Default.aspx", context.Object);

      // Assert
      Assert.True(defaultPage.Context.Items.Contains("test"));

    }

  }

}
