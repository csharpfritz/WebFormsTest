using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Xunit;
using Xunit.Abstractions;
using Fritz.WebFormsTest.Web.Scenarios.ModelBinding;

namespace Fritz.WebFormsTest.Test
{

  [Collection("Precompiler collection")]
  public class ModelBindingFixture
  {
    private ITestOutputHelper _testHelper;

    public ModelBindingFixture(ITestOutputHelper helper)
    {
      _testHelper = helper;
    }

    [Fact]
    public void SimpleSelect()
    {

      // Arrange
      var expectedItems = Simple.SampleItems;

      // Act
      var sut = WebApplicationProxy.GetPageByLocation<Simple>("/Scenarios/ModelBinding/Simple.aspx");
      sut.RunToEvent(WebFormEvent.PreRender);        // Need to execute all events
      var outHTML = sut.RenderHtml();

      _testHelper.WriteLine(outHTML);

      // Assert
      foreach (var item in expectedItems)
      {
        Assert.True(CanLocateTableWithCellContents(outHTML, "myGrid", item.ID.ToString()), $"Cannot locate myGrid with item.id={item.ID}");
        Assert.True(CanLocateTableWithCellContents(outHTML, "myGrid", item.Name), $"Cannot locate myGrid with item.name={item.Name}");
      }


    }

    [Fact]
    public void VerifyUpdateSetup()
    {

      // Arrange
      var sut = WebApplicationProxy.GetPageByLocation<EditForm>(
        "/Scenarios/ModelBinding/EditForm/1");
      var postData = new NameValueCollection();
      const string newName = "TestTwo";
      postData.Add("myForm$name", newName);

      // Act
      //sut.MockPostData(postData);
      sut.RunToEvent(WebFormEvent.PreRender);
      var results = sut.RenderHtml();


      // Assert
      //_testHelper.WriteLine(sut.Request.GetRouteData());
      

    }


    [Fact]
    public void SimpleUpdate()
    {

      // Arrange
      var sut = WebApplicationProxy.GetPageByLocation<EditForm>(
        "/Scenarios/ModelBinding/EditForm/1");
      var postData = new NameValueCollection();
      const string newName = "TestTwo";
      postData.Add("myForm$name", newName);


      // NOTE: Need to set the PostData but load values into the ModelState dictionary at Page.ModelState
      //

      // Act
      sut.MockPostData(postData);
      sut.RunToEvent(WebFormEvent.PreRender);
      //((FormView)(sut.FindControl("myForm"))).FindControl("Save").FireEvent("Command");
      //sut.FindControl<Button>("myForm","Save").FireEvent("Command");

      var form = sut.FindControl("myForm");
      var saveButton = form.FindControlHierarchical<Button>("Save");
      saveButton.FireEvent("Command", new CommandEventArgs("Update",""));

      // Assert
      Assert.Equal(newName, EditForm.SampleItems.First(i => i.ID == 1).Name);

    }

    private bool CanLocateTableWithCellContents(string html, string tableId, string valueToLocate)
    {

      var pattern = $@"<table.*?id=""{tableId}"".*?>([\s\S]*?)<\/table>";
      var reTable = new Regex(pattern, RegexOptions.Multiline);

      var tableHTML = reTable.Match(html).Value;
      if (tableHTML == null || tableHTML.Length == 0) return false;

      return tableHTML.Contains($">{valueToLocate}</td>");

    }
  }

}
