using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Fritz.WebFormsTest.Internal
{

  public class FriendlyUrlsHandler
  {

    private readonly List<string> _Segments = new List<string>();

    public bool IsFriendlyUrl { get; private set; }

    public string ConvertRequestUrlToFileLocation(string requestUrl) {

      IsFriendlyUrl = false;
      
      var legitExtensions = new[] { ".aspx", ".master", ".ascx", ".ashx", ".asmx" };

      // Return immediately if this already has an ASPX in it
      if (legitExtensions.Any(e => requestUrl.ToLowerInvariant().Contains(e)))
        return requestUrl;

      var folders = requestUrl.Split('/');
      var currentLocation = "";
      var foundLocation = false;
      var mpProvider = new TestConfigMapPath();
      foreach (var folder in folders)
      {

        if (string.IsNullOrEmpty(folder)) continue;

        if (foundLocation) {
          _Segments.Add(folder);
          continue;
        }

        currentLocation += "/" + folder;
        var absoluteLocation = mpProvider.MapPath("", currentLocation);
        if (Directory.Exists(absoluteLocation)) continue;
        if (File.Exists(absoluteLocation + ".aspx"))
        {
          foundLocation = true;
          continue;
        }

        throw new FileNotFoundException("Unable to locate the file requested", requestUrl);

      }

      IsFriendlyUrl = true;

      return currentLocation + ".aspx";

    }

    internal void LogInformationToContext(HttpContext context)
    {

      if (!IsFriendlyUrl) return;

      var requestUrl = context.Request.Url.ToString();
      var newRouteData = new RouteData();
      newRouteData.DataTokens[FriendlyUrlSegmentsKey] = _Segments;
      context.Items[RouteDataItemsKey] = newRouteData;

    }

    internal string FriendlyUrlSegmentsKey {
      get {

        var type = typeof(Microsoft.AspNet.FriendlyUrls.HttpRequestExtensions);
        var theConst = type.GetField("FriendlyUrlSegmentsKey", BindingFlags.Static | BindingFlags.NonPublic);
        return theConst.GetValue(null).ToString();

      }
    }

    internal object RouteDataItemsKey {
      get {

        var type = typeof(Microsoft.AspNet.FriendlyUrls.FriendlyUrl)
          .Assembly.GetType("Microsoft.AspNet.FriendlyUrls.FriendlyUrlsModule");
        var theField = type.GetField("RouteDataItemsKey", BindingFlags.NonPublic | BindingFlags.Static);
        return theField.GetValue(null);

      }
    }
  }

}
