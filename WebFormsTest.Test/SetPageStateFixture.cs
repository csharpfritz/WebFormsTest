using Fritz.WebFormsTest.Web.Scenarios.Postback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Web.UI.WebControls;

namespace Fritz.WebFormsTest.Test
{

  [Collection("Precompiler collection")]
  public class SetPageStateFixture
  {

    [Fact]
    public void VerifyControlIsConfiguredProperly()
    {

      // Arrange
      var sut = WebApplicationProxy.GetPageByLocation<Textbox_StaticId>("/Scenarios/Postback/Textbox_StaticId.aspx");

      var EXPECTED = "Configured Text";

      // Act
      sut.SetPageState<TextBox>("TestTextboxControl", tb =>
      {
        tb.Text = EXPECTED;
      });

      // Assert
      var thisTb = sut.FindControl("TestTextboxControl") as TextBox;
      Assert.Equal(EXPECTED, thisTb.Text);

    }

  }

}
