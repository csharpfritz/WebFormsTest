using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fritz.WebFormsTest
{

  /// <summary>
  /// A collection of extension methods to enhance the testability of WebControls
  /// </summary>
  public static class ControlExtensions
  {

    public static void FireEvent(this Control ctrl, string eventName, EventArgs args = null)
    {

      // Set the default EventArgs if no value was submitted
      if (args == null) args = EventArgs.Empty;

      // Locate the event
      var ei = ctrl.GetType().GetEvent(eventName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);

      var raiseMethod = ei.GetRaiseMethod(true);
      if (raiseMethod != null)
      {
        raiseMethod.Invoke(ctrl, new object[] { ctrl, args });
        return;
      }

      var onMethod = ctrl.GetType().GetMethod("On" + eventName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.NonPublic);
      if (onMethod != null)
      {
        onMethod.Invoke(ctrl, new object[] { args });
        return;
      }

    }


  }

}
