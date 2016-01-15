using Fritz.WebFormsTest.Web;
using Fritz.WebFormsTest.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using Xunit;

namespace Fritz.WebFormsTest.Test
{

  /// <summary>
  /// A collection of tests to verify the Cache is available and functioning
  /// </summary>
  [Collection("Precompiler collection")]
  public class CacheFixture : BaseFixture
  {

    [Fact]
    public void IsAvailable()
    {

      // Arrange
      //var cache = WebApplicationProxy.Cache;
      var TEST_VALUE = new Uri("http://www.github.com");
      //cache.Insert("url", TEST_VALUE);
      //context.SetupGet(ctx => ctx.Cache).Returns(cache);

      // Act
      var sut = WebApplicationProxy.GetPageByLocation<_Default>("/default.aspx");
      HttpContext.Current.Cache.Insert("url", TEST_VALUE);
      //sut.Context = context.Object;

      // Assert
      Assert.NotNull(sut.Context().Cache.Get("url"));
      Assert.Equal(TEST_VALUE, sut.Context().Cache.Get("url"));

    }

  }
}
