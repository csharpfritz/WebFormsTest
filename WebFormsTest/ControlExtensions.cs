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

      if (ei == null) throw new ArgumentException($"Cannot locate the event '{eventName}' on the control");

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

      // Should only reach here if the event handlers raise method was not found for the event submitted
      throw new ArgumentException($"Unable to find a suitable raise method to trigger the event {eventName}");

    }


  }

}
