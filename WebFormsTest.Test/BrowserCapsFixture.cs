using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace Fritz.WebFormsTest.Test
{

  /// <summary>
  /// A fixture to verify that the browserCaps features of the framework are functioning
  /// </summary>
  public class BrowserCapsFixture
  {

    [Fact]
    public void VerifyIE11()
    {

      // Arrange

      // Act
      var sut = BrowserDefinitions.IE11;

      // Assert
      Assert.NotNull(sut);
      Assert.Equal("InternetExplorer", sut.Browser);
      Assert.Equal(11, sut.MajorVersion);
      Assert.Equal(new Version("3.0"), sut.EcmaScriptVersion);
      Assert.False(sut.IsMobileDevice);

    }

    [Fact]
    public void VerifyChrome49()
    {

      // Arrange

      // Act
      HttpBrowserCapabilities sut = BrowserDefinitions.Chrome49;

      // Assert
      Assert.NotNull(sut);
      Assert.Equal("Chrome", sut.Browser);
      Assert.Equal(49, sut.MajorVersion);
      Assert.False(sut.IsMobileDevice);

    }

    [Fact]
    public void VerifyFirefox45()
    {

      // Arrange

      // Act
      HttpBrowserCapabilities sut = BrowserDefinitions.Firefox45;

      //Assert
      Assert.NotNull(sut);
      Assert.Equal("Firefox", sut.Browser);
      Assert.Equal(45, sut.MajorVersion);
      Assert.False(sut.IsMobileDevice);


    }

    [Fact]
    public void VerifyMobileSafari7()
    {

      // Arrange

      // Act
      HttpBrowserCapabilities sut = BrowserDefinitions.MobileSafari7;


      // Assert
      Assert.NotNull(sut);
      Assert.Equal("Safari", sut.Browser);
      Assert.Equal(7, sut.MajorVersion);
      Assert.True(sut.IsMobileDevice);

    }

    [Fact]
    public void VerifyIPhoneChrome()
    {

      // Arrange

      // Act
      HttpBrowserCapabilities sut = BrowserDefinitions.IPhoneChrome;

      // Assert
      Assert.NotNull(sut);
      Assert.Equal("Safari", sut.Browser);
      Assert.Equal(0, sut.MajorVersion);
      Assert.True(sut.IsMobileDevice);

    }

  }

}
