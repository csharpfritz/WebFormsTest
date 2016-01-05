using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Fritz.WebFormsTest.Test.Internal
{

  /// <summary>
  /// A collection of tests to verify the operation of the VirtualPathProxy
  /// </summary>
  [Collection("Precompiler collection")]
  public class VirtualPathFixture
  {
    private ITestOutputHelper _helper;

    public VirtualPathFixture(ITestOutputHelper helper)
    {
      _helper = helper;
    }

    [Fact]
    public void CanMapScenarios()
    {

      // Arrange
      var scenariosFolder = "~/Scenarios";

      // Act
      var result = VirtualPathWrapper.Create(scenariosFolder);

      // Assert
      _helper.WriteLine(result.VirtualPathString);

    }

  }

}
