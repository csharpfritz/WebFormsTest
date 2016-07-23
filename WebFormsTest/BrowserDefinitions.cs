using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Configuration;
using Fritz.WebFormsTest.Internal;
using System.Collections.Specialized;

namespace Fritz.WebFormsTest
{

  /// <summary>
  /// A collection of different browser definitions as browser capabilities
  /// </summary>
  public static class BrowserDefinitions
  {

    private static readonly BrowserCapabilitiesFactory _BrowserCapsFactory = new BrowserCapabilitiesFactory();
    private static readonly NameValueCollection _DefaultHeaders = new NameValueCollection();

    public static HttpBrowserCapabilities DEFAULT
    {
      get
      {
        var caps = new HttpBrowserCapabilities();

        IDictionary items = new Dictionary<string, object>();
        items.Add("cookies", "false");
        items.Add("ecmascriptversion", "3.0.0"); // Everyone has 5 for now...
        items.Add("preferredRenderingMime", "text/html");
        items.Add("preferredRequestEncoding", "UTF-8");
        items.Add("preferredResponseEncoding", "UTF-8");
        items.Add("requiresXhtmlCssSuppression", "false");
        items.Add("w3cdomversion", "1.0.0");

        caps.SetItems(items);

        return caps;

      }
    }

    public static HttpBrowserCapabilities IE11 {
      get
      {
        return ConfigureBasedOnAgentString(@"Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; AS; rv:11.0) like Gecko");
      }
    }

    public static HttpBrowserCapabilities Chrome49 {
      get
      {

        return ConfigureBasedOnAgentString(@"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36");

      }
    }


    public static HttpBrowserCapabilities Firefox45
    {
      get
      {
        return ConfigureBasedOnAgentString(@"Mozilla/5.0 (Windows NT 10.0; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0");
      }
    }

    /// <summary>
    /// Mobile Safari Browser on an iPhone
    /// </summary>
    public static HttpBrowserCapabilities MobileSafari7
    {
      get
      {
        return ConfigureBasedOnAgentString(@"Mozilla/5.0 (iPhone; CPU iPhone OS 7_0_4 like Mac OS X) AppleWebKit/537.51.1 (KHTML, like Gecko) Version/7.0 Mobile/11B554a Safari/9537.53");
      }
    }

    public static HttpBrowserCapabilities IPhoneChrome
    {
      get
      {
        return ConfigureBasedOnAgentString(@"Mozilla/5.0 (iPhone; U; CPU iPhone OS 5_1_1 like Mac OS X; en) AppleWebKit/534.46.0 (KHTML, like Gecko) CriOS/19.0.1084.60 Mobile/9B206 Safari/7534.48.3");
      }
    }


    private static HttpBrowserCapabilities ConfigureBasedOnAgentString(string agentString) {

      var caps = new HttpBrowserCapabilities();
      IDictionary items = new Dictionary<string, object>();
      items.Add(string.Empty, agentString);
      caps.SetItems(items);

      _BrowserCapsFactory.ConfigureBrowserCapabilities(_DefaultHeaders, caps);
      return caps;

    }

  }

}
