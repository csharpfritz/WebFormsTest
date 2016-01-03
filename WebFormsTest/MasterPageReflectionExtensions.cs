using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Fritz.WebFormsTest
{

  /// <summary>
  /// A collection of extension methods for a MasterPage that provide access to non-public methods with reflection
  /// </summary>
  internal static class MasterPageReflectionExtensions
  {

    public static void SetContentTemplates(this MasterPage master, IDictionary contentTemplateCollection)
    {

      var f = typeof(MasterPage).GetField("_contentTemplates", BindingFlags.NonPublic | BindingFlags.Instance);
      f.SetValue(master, contentTemplateCollection);

    }

    public static void SetOwnerControl(this MasterPage master, TemplateControl owner)
    {

      var f = typeof(MasterPage).GetField("_ownerControl", BindingFlags.NonPublic | BindingFlags.Instance);
      f.SetValue(master, owner);

    }


  }

}
