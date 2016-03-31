using Fritz.WebFormsTest.Web.Scenarios.Postback;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Xunit;
using Xunit.Abstractions;

namespace Fritz.WebFormsTest.Test
{

  /// <summary>
  /// A fixture that will inspect and test the postback handling of controls with Static client ids
  /// </summary>
  [Collection("Precompiler collection")]
  public class Postback_Simple_Fixture : BaseFixture
  {

    private readonly ITestOutputHelper output;

    public Postback_Simple_Fixture(ITestOutputHelper output, PrecompiledWebConfiguration precompiler)
    {
      this.output = output;
    }

    [Fact]
    public void TextboxAddedToControlSet()
    {

      // Arrange

      // Act
      var sut = WebApplicationProxy.GetPageByLocation<Textbox_StaticId>("/Scenarios/Postback/Textbox_StaticId.aspx");
      sut.FireEvent(WebFormEvent.Init, EventArgs.Empty);

      // Assert
      Assert.NotEqual(0, sut.Controls.Count);
      Assert.NotNull(sut.FindControl("TestTextboxControl"));

      TextBox tb = sut.FindControl("TestTextboxControl") as TextBox;
      output.WriteLine("Textbox found with text: " + tb.Text);
      Assert.Equal("Initial Text", tb.Text);

    }

    [Fact]
    public void IsPostbackShouldBeTrue()
    {

      // Arrange
      var postData = new NameValueCollection();

      // Act
      var sut = WebApplicationProxy.GetPageByLocation<Textbox_StaticId>("/Scenarios/Postback/Textbox_StaticId.aspx");
      sut.MockPostData(postData);
      sut.RunToEvent(WebFormEvent.PreRender);

      // Assert
      Assert.True(sut.IsPostBack, "Page 'IsPostBack' is not set to true");

    }

    [Fact]
    public void TextboxContentChanged()
    {

      // Arrange
      var postData = new NameValueCollection();
      const string NEW_TEXT = "Posted back";
      postData.Add("TestTextboxControl", NEW_TEXT);

      // Act
      var sut = WebApplicationProxy.GetPageByLocation<Textbox_StaticId>("/Scenarios/Postback/Textbox_StaticId.aspx");
      sut.MockPostData(postData);
      sut.RunToEvent(WebFormEvent.PreRender);

      // Assert
      Assert.Equal(NEW_TEXT, sut.TestControl.Text);
    }

  }

}
