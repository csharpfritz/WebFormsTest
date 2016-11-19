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

    /// <summary>
    /// Return a child control of the type specified with the id requested
    /// </summary>
    /// <typeparam name="T">Type of the control saught</typeparam>
    /// <param name="ctl">The control to search through</param>
    /// <param name="id">ID of the control to find</param>
    /// <returns></returns>
    public static T FindControl<T>(this Control ctl, string id) where T : Control
    {

      return ctl.FindControl(id) as T;

    }

    /// <summary>
    /// Find a control of the type specified navigating the hierarchy of control ids submitted
    /// </summary>
    /// <typeparam name="T">The type of the control sought</typeparam>
    /// <param name="ctl">The control whose child-controls should be sought</param>
    /// <param name="args">The hierarchy of control ids to spelunk through</param>
    /// <returns></returns>
    public static T FindControl<T>(this Control ctl, params string[] args) where T : Control
    {

      T foundControl = null;
      foreach (var arg in args)
      {
        foundControl = ctl.FindControl<T>(arg);
        if (foundControl == null) break;
      }

      return foundControl as T;

    }

    public static T FindControlHierarchical<T>(this Control ctl, string id) where T : Control {

      var foundControl = ctl.FindControl<T>(id);
      if (foundControl != null) return foundControl;

      foreach (var c in ctl.Controls)
      {

        var thisCtl = c as Control;
        if (thisCtl == null) continue;

        if (thisCtl.HasControls()) {
          foundControl = thisCtl.FindControlHierarchical<T>(id);
          if (foundControl != null) break;
        }

      }

      return foundControl;

    }

  }

}
