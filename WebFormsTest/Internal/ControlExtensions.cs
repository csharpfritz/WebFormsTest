using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Web.UI
{
  public static class ControlExtensions
  {

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

  }
}
