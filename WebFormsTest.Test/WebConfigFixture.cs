using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Fritz.WebFormsTest.Test
{

  /// <summary>
  /// Verify that the web application can still access web.config
  /// </summary>
  [Collection("Precompiler collection")]
  public class WebConfigFixture
  {

    // TEMPORARILY DISABLING THESE TESTS WHILE RESEARCHING THE PERMISSION PROBLEM

    public WebConfigFixture(ITestOutputHelper output)
    {
      this.Output = output;
    }

    public ITestOutputHelper Output { get; }

    [Fact(Skip ="Config inspection is unreliable in integration tests")]
    public void CanFetchAppSettings()
    {

      // Arrange
      var sut = WebApplicationProxy.GetPageByLocation<Web.Scenarios.WebConfig.AppSettings>("/Scenarios/WebConfig/AppSettings.aspx");
      var expectedValue = "ThisIsMyTestSettingValue";

      // Act
      var extractedSetting = sut.TestConfigValue;

      Output.WriteLine($"Found this config value: '{extractedSetting}'");

      // Assert
      Assert.Equal(expectedValue, extractedSetting);

    }

    [Fact(Skip = "Config inspection is unreliable in integration tests")]
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
