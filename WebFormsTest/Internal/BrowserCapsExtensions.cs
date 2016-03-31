using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace Fritz.WebFormsTest.Internal
{
  internal static class BrowserCapsExtensions
  {

    internal static void SetItems(this HttpBrowserCapabilities caps, IDictionary items)
    {

      var theField = typeof(HttpCapabilitiesBase).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
      theField.SetValue(caps, items);

      var browserCaps = new HttpBrowserCapabilities();

    }

  }

}
