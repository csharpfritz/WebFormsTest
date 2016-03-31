using Fritz.WebFormsTest.Web.Scenarios.ControlTree;
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
  public class ControlFireEventFixture 
  {

    [Fact]
    public void FireTriggersClickEvent()
    {

      // Arrange
      var page = WebApplicationProxy.GetPageByLocation<Buttons_AutoId>("/Scenarios/ControlTree/Buttons_AutoId.aspx");

      // Run to PreRender, the buttons are in a FormView that is databound in Load
      page.RunToEvent(WebFormEvent.PreRender);

      // Act
      var sut = page.ButtonA;
      sut.FireEvent("Click", null);

      // Assert
      Assert.False(sut.Enabled);

    }

    [Fact]
    public void FireTriggersCommandEvent()
    {

      // Arrange
      var page = WebApplicationProxy.GetPageByLocation<Buttons_AutoId>("/Scenarios/ControlTree/Buttons_AutoId.aspx");

      // Run to PreRender, the buttons are in a FormView that is databound in Load
      page.RunToEvent(WebFormEvent.PreRender);

      // Act
      var sut = page.ButtonA;
      const string TEST_CAPTION = "This is a test";
      var args = new CommandEventArgs("Caption", TEST_CAPTION);
      sut.FireEvent("Command", args);

      // Assert
      Assert.Equal(TEST_CAPTION, sut.Text);

    }

  }
}
