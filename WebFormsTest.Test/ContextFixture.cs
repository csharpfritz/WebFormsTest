using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fritz.WebFormsTest.Test
{


    [Collection("Precompiler collection")]
    public class ContextFixture
    {

        [Fact]
        public void DetectRedirect()
        {

            // Arrange

            // Act
            var sut = WebApplicationProxy.GetPageByLocation<Web.Scenarios.Context.ResponseRedirect>("/Scenarios/Context/ResponseRedirect.aspx");
            sut.RunToEvent(WebFormEvent.PreRender);

            // Assert
            Assert.Equal("/default.aspx", sut.Response.RedirectLocation);


        }

    }
}
