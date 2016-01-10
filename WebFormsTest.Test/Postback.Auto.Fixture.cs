using Fritz.WebFormsTest.Web.Scenarios.ControlTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Fritz.WebFormsTest.Test
{

  [Collection("Precompiler collection")]
  public class Postback_Auto_Fixture : BaseFixture
  {

    private readonly ITestOutputHelper output;

    public Postback_Auto_Fixture(ITestOutputHelper output, PrecompilerFixture precompiler)
    {
      this.output = output;
    }

    [Fact]
    public void ClientIdDoesNotMatchControlId()
    {

      // Arrange

      // Act
      var sut = WebApplicationProxy.GetPageByLocation<Buttons_AutoId>("/Scenarios/ControlTree/Buttons_AutoId.aspx", context.Object);

      // Run to PreRender, the buttons are in a FormView that is databound in Load
      sut.RunToEvent(TestablePage.WebFormEvent.PreRender);

      // Assert
      Assert.NotEqual("buttonA", sut.ButtonA.ClientID);
      Assert.NotEqual("buttonB", sut.ButtonB.ClientID);

    }

  }
}
