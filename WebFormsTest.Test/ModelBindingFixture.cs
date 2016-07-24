using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Xunit;
using Xunit.Abstractions;

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
            var expectedItems = WebFormsTest.Web.Scenarios.ModelBinding.Simple.SampleItems;

            // Act
            var sut = WebApplicationProxy.GetPageByLocation<Web.Scenarios.ModelBinding.Simple>("/Scenarios/ModelBinding/Simple.aspx");
            sut.RunToEvent(WebFormEvent.PreRender);        // Need to execute all events
            var outHTML = sut.RenderHtml();

            _testHelper.WriteLine(outHTML);

            // Assert
            foreach (var item in expectedItems)
            {
                Assert.True(CanLocateTableWithCellContents(outHTML, "myGrid", item.ID.ToString()));
                Assert.True(CanLocateTableWithCellContents(outHTML, "myGrid", item.Name));
            }


        }

        [Fact]
        public void SimpleUpdate()
        {

            // Arrange
            var sut = WebApplicationProxy.GetPageByLocation<Web.Scenarios.ModelBinding.EditForm>("/Scenarios/ModelBinding/EditForm/1");
            var postData = new NameValueCollection();
            const string newName = "TestTwo";
            postData.Add("myForm$name", newName);

            // Act
            sut.MockPostData(postData);
            ((FormView)(sut.FindControl("myForm"))).FindControl("Save").FireEvent("Command");

            // Assert
            Assert.Equal(newName, Web.Scenarios.ModelBinding.EditForm.SampleItems.First(i => i.ID == 1).Name);

        }

        private bool CanLocateTableWithCellContents(string html, string tableId, string valueToLocate)
        {

            var pattern = $@"<table.*?id=""{tableId}"".*?>([\s\S]*?)<\/table>";
            var reTable = new Regex(pattern, RegexOptions.Multiline);

            var tableHTML = reTable.Match(html).Value;
            if (tableHTML == null || tableHTML.Length == 0) return false;

            return tableHTML.Contains($"<td>{valueToLocate}</td>");

        }
    }

}
