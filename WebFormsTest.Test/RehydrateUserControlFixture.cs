using Fritz.WebFormsTest.Web.Scenarios.ControlTree;
using Fritz.WebFormsTest.Web.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Xunit;

namespace Fritz.WebFormsTest.Test
{


  [Collection("Precompiler collection")]
  public class RehydrateUserControlFixture : BaseFixture
  {


    /// <summary>
    /// Inspect the user control to verify properties are properly set
    /// </summary>
    [Fact]
    public void OriginalValueSet()
    {

      // Arrange

      // Act
      var sut = WebApplicationProxy.GetPageByLocation<PageWithUserControl>("/Scenarios/ControlTree/PageWithUserControl.aspx");
      sut.Context = context.Object;
      sut.PrepareTests();
      sut.RunToEvent(TestablePage.WebFormEvent.PreRender);

      // Assert
      var theControl = sut.FindControl("MyLinkControl") as MyLinkControl;
      var theLink = theControl.FindControl("myLink") as LinkButton;
      Assert.Equal("New Text", theControl.LinkButtonText);
      Assert.Equal("DoStuff", theLink.CommandName);

    }

  }

}
