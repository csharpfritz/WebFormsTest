using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WebFormsInspect.Test
{

  /// <summary>
  /// A fixture that will inspect and test the postback handling of controls with Static client ids
  /// </summary>
  public class Postback_Simple_Fixture : BaseFixture
  {

    [Fact]
    public void TextboxReloaded()
    {

      // Arrange
      var textBoxContent = "This is my test " + new Random().Next(1, 1000);
      var form = new NameValueCollection();
      form.Add("textbox", textBoxContent);

      // Act
      var sut = new Postback_Simple
      {
        Context = context.Object
      };
      sut.MockPostData(form);

      // Assert
      //Assert.True(sut.IsPostBack, "Postback was not flagged properly");
      Assert.Equal(textBoxContent, sut.Textbox.Text);

    }

  }

}
