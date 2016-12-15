using Fritz.WebFormsTest;
using Fritz.WebFormsTest.Internal;
using Fritz.WebFormsTest.Test;
using Fritz.WebFormsTest.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;
using System.Web.Routing;
using Fritz.WebFormsTest.Web.Scenarios.WebServices;
using Xunit;
using Xunit.Abstractions;

namespace Fritz.WebFormsTest.Test
{
  [Collection("Precompiler collection")]
  public class WebApplicationProxyFixture 
  {

    private PrecompiledWebConfiguration _Fixture;
    private ITestOutputHelper _testHelper;

    public WebApplicationProxyFixture(PrecompiledWebConfiguration fixture, ITestOutputHelper helper)
    {
      _Fixture = fixture;

      _testHelper = helper;
      _testHelper.WriteLine("Target folder: " + WebApplicationProxy.WebRootFolder);
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

    [Fact]
    public void MasterPageShouldHaveSameHttpContextAsPage()
    {
      HttpContext context = null;
      WebApplicationProxy.GetPageByLocation<_Default>("/Default.aspx", ctx =>
      {
        // Grab the context from the modifier so that we can check it later after the GetPageByLocation for default is finished.
        context = ctx;
      });
			
      Assert.Equal(HttpContext.Current, context);
    }

    [Fact]
    public void HttpApplicationStateShouldNotBeNull()
    {

      // Arrange

      // Act


      // Assert
      Assert.NotNull(WebApplicationProxy.Application.Application);

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
      Action<HttpContext> addContext = new Action<HttpContext>(ctx => ctx.Items.Add("test", "test"));
      var defaultPage = WebApplicationProxy.GetPageByLocation<_Default>("/Default.aspx", addContext);

      // Act
      //HttpContext.Current.Items.Add("test", "test");
      //var myDict = new Dictionary<string, string>();
      //myDict.Add("test", "test");
      //base.context.SetupGet(c => c.Items).Returns(myDict);

      // Assert
      Assert.True(defaultPage.Context().Items.Contains("test"));

    }

    [Fact]
    public void SupportBundleConfig()
    {

      // Arrange

      // Act
      BundleConfig.RegisterBundles(BundleTable.Bundles);

      // Assert

    }

    [Fact]
    public void HostingVirtualPathProviderIsSet()
    {

      Assert.NotNull(HostingEnvironment.VirtualPathProvider);

    }

    [Fact]
    public void FriendlyUrlIsHandled()
    {

      // Arrange
      var expectedType = typeof(WebFormsTest.Web.Scenarios.Postback.Textbox_StaticId);

      // Act

      // Get the default page
      var locatedType = WebApplicationProxy.GetPageByLocation("/Scenarios/Postback/Textbox_StaticId").GetType();
      _testHelper.WriteLine("Type returned: " + locatedType.BaseType.FullName);

      // NOTE: This needs to look at the BaseType of the type returned from the GetPageByLocation
      // because this is the JIT'd page which is merged with the ASPX and inherits from the type
      // defined in the code-behind

      // Assert
      Assert.Equal(expectedType.FullName, locatedType.BaseType.FullName);

    }

  }

}
