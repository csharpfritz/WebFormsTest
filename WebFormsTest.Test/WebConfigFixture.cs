using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fritz.WebFormsTest.Test
{

  /// <summary>
  /// Verify that the web application can still access web.config
  /// </summary>
  [Collection("Precompiler collection")]
  public class WebConfigFixture
  {

    [Fact]
    public void CanFetchAppSettings()
    {

      // Arrange
      var sut = WebApplicationProxy.GetPageByLocation<Web.Scenarios.WebConfig.AppSettings>("/Scenarios/WebConfig/AppSettings.aspx");
      var expectedValue = "ThisIsMyTestSettingValue";

      // Act
      var extractedSetting = sut.TestConfigValue;

      // Assert
      Assert.Equal(expectedValue, extractedSetting);

    }

    [Fact]
    public void CanFetchConnectionStrings()
    {

      // Arrange
      var expectedValue = "aspnet-WebFormsTest-20151207082329";

      // Act
      var sut = WebApplicationProxy.GetPageByLocation<Web.Scenarios.WebConfig.AppSettings>("/Scenarios/WebConfig/AppSettings.aspx");
      var extractedSetting = sut.TestInitialCatalog;

      // Assert
      Assert.Equal(expectedValue, extractedSetting);


    }


  }

}
