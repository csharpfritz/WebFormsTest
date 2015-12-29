using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace WebFormsInspect.Test
{

  /// <summary>
  /// A fixture that will inspect and test the postback handling of controls with Static client ids
  /// </summary>
  public class Postback_Simple_Fixture : BaseFixture
  {

    private readonly ITestOutputHelper output;

    public Postback_Simple_Fixture(ITestOutputHelper output)
    {
      this.output = output;
    }

    [Fact]
    public void TextboxAddedToControlSet()
    {

      // Arrange

      // Act
      var sut = new Scenarios.Postback.Textbox_StaticId
      {
        Context = context.Object
      };
      sut.FireEvent(WebFormsTest.TestablePage.WebFormEvent.Init, EventArgs.Empty);

      // Assert
      Assert.NotEqual(0, sut.Controls.Count);


    }

  }

}
