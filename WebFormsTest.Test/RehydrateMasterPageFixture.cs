using Fritz.WebFormsTest;
using Fritz.WebFormsTest.Web;
using Fritz.WebFormsTest.Web.Scenarios.Postback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xunit;
using Xunit.Abstractions;

namespace Fritz.WebFormsInspect.Test
{

  /// <summary>
  /// A fixture that will inspect to ensure that the MasterPage is hydrated properly in a page
  /// </summary>
  [Collection("Precompiler collection")]
  public class RehydrateMasterPageFixture 
  {
    private ITestOutputHelper _helper;

    public RehydrateMasterPageFixture(ITestOutputHelper helper)
    {
      _helper = helper;
    }

    [Fact]
    public void CanIgetMasterPage()
    {

      var sut = WebApplicationProxy.GetPageByLocation("/Site.Master") as SiteMaster;

      Assert.NotNull(sut);


    }

    /// <summary>
    /// Verify that MasterPages are created properly
    /// </summary>
    [Fact]
    public void MasterPageIsCreated()
    {

      // Arrange

      // Act
      var sut = WebApplicationProxy.GetPageByLocation<_Default>("/default.aspx");
      sut.Context = new TestablePage.EmptyHttpContext();

      // Assert
      Assert.NotNull(sut.Master);
      _helper.WriteLine("Master Page Cheeseburger: " + ((SiteMaster)sut.Master).cheeseburger);

    }


    /// <summary>
    /// This is a discovery unit test, helping to verify that MasterPageFile is null when the page directive is not set
    /// </summary>
    [Fact]
    public void WhatIsMasterPageFileInPageNotUsingMasterPage()
    {

      var sut = WebApplicationProxy.GetPageByLocation<Textbox_StaticId>("/Scenarios/Postback/Textbox_StaticId.aspx");

      _helper.WriteLine("MasterPageFile: " + sut.MasterPageFile);

      Assert.Null(sut.MasterPageFile);


    }

  }
}
