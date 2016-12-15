using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace Fritz.WebFormsTest.Internal
{

  internal static class InternalPageExtensions
  {

    private static readonly Type _PageType = typeof(Page);

    public static IDictionary get_ContentTemplateCollection(this Page myPage)
    {

      var p = _PageType.GetField("_contentTemplateCollection", BindingFlags.NonPublic | BindingFlags.Instance);
      return p.GetValue(myPage) as IDictionary;

    }


    /// <summary>
    /// Prepare the page for testing by triggering all of the setup methods that a Page would normally have triggered when running on a web server
    /// </summary>
    internal static void PrepareForTest(this Page myPage)
    {

      var setIntMethod = _PageType.GetMethod("SetIntrinsics", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(HttpContext) }, null);
      setIntMethod.Invoke(myPage, new object[] { HttpContext.Current });

      // NOTE: This is a COMPLETE fake out and wrap around the generated code
      var initMethod = _PageType.GetMethod("FrameworkInitialize", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.FlattenHierarchy);
      initMethod.Invoke(myPage, null);

      // TODO: Add method for this
      var hookupMethod = typeof(TemplateControl).GetMethod("HookUpAutomaticHandlers", BindingFlags.NonPublic | BindingFlags.Instance);
      hookupMethod.Invoke(myPage, null);

    // Disable EventValidation
      myPage.EnableEventValidation = false;

      var preInit = _PageType.GetMethod("OnPreInit", BindingFlags.NonPublic | BindingFlags.Instance);
      preInit.Invoke(myPage, new object[] { EventArgs.Empty });

      // Grab a masterPage if in use
      AddMasterPage(myPage);

      var initRecursive = _PageType.GetMethod("InitRecursive", BindingFlags.Instance | BindingFlags.NonPublic);
      initRecursive.Invoke(myPage, new object[] { null });

    }

    private static void AddMasterPage(Page myPage)
    {

      if (string.IsNullOrEmpty(myPage.MasterPageFile)) return;

      // Prevent the master page from changing the HttpContext.
      var master = WebApplicationProxy.GetPageByLocation(myPage.MasterPageFile, modifyExistingContext: false) as MasterPage;
      var masterField = typeof(Page).GetField("_master", BindingFlags.Instance | BindingFlags.NonPublic);
      masterField.SetValue(myPage, master);

      // Initialize the rest of the junk on page for the Master
      if (myPage.HasControls()) myPage.Controls.Clear();
      var contentTemplates = ((Page)myPage).get_ContentTemplateCollection();
      master.SetContentTemplates(contentTemplates);
      master.SetOwnerControl((Page)myPage);
      master.InitializeAsUserControl(myPage.Page);
      myPage.Controls.Add(master);

    }

    internal static HttpContext Context(this Page myPage)
    {

      return _PageType.GetProperty("Context", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(myPage, null) as HttpContext;

    }


  }

}
