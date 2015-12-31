using System;
using System.ComponentModel;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Diagnostics;

namespace Fritz.WebFormsTest
{
  public class TestablePage : Page
  {

    /**
    NOTES:

  Page.ProcessPostData 
---- Loads data from Postback
---- Need to have controls constructed before the Postdata can be loaded
---- Need to load controls with the same client-side-id that they were assigned ?? how do we identify these?


**/

    public const string COMMENT_MARKER = "<!-- From a testable web page -->";
    private HttpContextBase _Context;
    private static readonly BindingFlags AllBindings = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private readonly AutoEventHandler _AutoEventHandler;
    private EmptyTestServer _TestServer;

    public enum WebFormEvent
    {
      Init,
      Load,
      PreRender,
      Unload
    }

    public TestablePage()
    {

      if (IsInTestMode)
      {
        // Reference the empty context here in case any nasty framework items are looking for it before a mock is provided
        Context = new EmptyHttpContext();
      }

    }

    public void PrepareTests()
    {

      // This is a test-only concern
      if (!IsInTestMode) return;

      // NOTE: This is a COMPLETE fake out and wrap around the generated code
      base.FrameworkInitialize();
      var mi = this.GetType().GetMethod("__BuildControlTree", BindingFlags.NonPublic | BindingFlags.Instance);
      mi.Invoke(this, new object[] { this });

      OnPreInit(EventArgs.Empty);

    }
    /// <summary>
    /// Folder location where the ASPX / ASCX files are stored so that the unit-test harness can load page and control content
    /// </summary>
    public static string WebRootFolder { get; set; }

    public new HttpContextBase Context
    {
      get
      {
        if (_Context == null) _Context = new HttpContextWrapper(HttpContext.Current);
        return _Context;
      }
      set
      {
        _Context = value;
      }
    }

    #region Methods / Properties to help test

    public void FireEvent(WebFormEvent e, EventArgs args)
    {

      switch (e)
      {
        case WebFormEvent.Init:
          this.OnInit(args);
          break;
        case WebFormEvent.Load:
          this.OnLoad(args);
          break;
        case WebFormEvent.PreRender:
          this.OnPreRender(args);
          break;
        case WebFormEvent.Unload:
          this.OnUnload(args);
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Helper property for inherited pages to indicate that we are in 'unit test mode'
    /// </summary>
    public static bool IsInTestMode
    {
      get { return HttpContext.Current == null; }
    }

    /// <summary>
    /// Render the HTML for this page using the Render method and return the rendered string
    /// </summary>
    /// <returns></returns>
    public string RenderHtml()
    {
      var sb = new StringBuilder();
      var txt = new HtmlTextWriter(new StringWriter(sb));
      base.Render(txt);
      return sb.ToString();
    }

    public void MockPostData(NameValueCollection postData)
    {

      // Ensure that the control structure is available
      // TODO: This is not actually loading the control hierarchy
      EnsureChildControls();

      // Load the data
      var hiddenMethod = typeof(Page).GetMethod("ProcessPostData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      hiddenMethod.Invoke(this, new object[] { postData, true });

    }


    /// <summary>
    /// Is the submitted method name implemented?
    /// </summary>
    /// <param name="methodName"></param>
    /// <returns></returns>
    private bool IsMethodPresent(string methodName)
    {
      return GetType().GetMethod(methodName, AllBindings) != null;
    }

    #endregion

    #region Replaced Properties that allow mocking Web Interactions

    public new HttpResponseBase Response
    {
      get { return Context.Response; }
    }

    public new HttpRequestBase Request
    {
      get { return Context.Request; }
    }

    public new HttpSessionStateBase Session
    {
      get { return Context.Session; }
    }

    public new HttpServerUtilityBase Server
    {
      get {

        if (IsInTestMode)
        {
          if (_TestServer == null) _TestServer = new EmptyTestServer();
          return _TestServer;
        }

        return Context.Server;

      }
    }

    #endregion

    protected override void OnPreRender(EventArgs e)
    {

      base.OnPreRender(e);

      if (Context.IsDebuggingEnabled)
      {
        this.Controls.Add(new LiteralControl(COMMENT_MARKER));
      }

    }

    protected override void CreateChildControls()
    {

      if (IsInTestMode)
      {
        Debug.WriteLine("Controls in collection: " + Controls.Count);
      } else
      {
        base.CreateChildControls();
      }

    }

    public override ControlCollection Controls
    {
      get
      {
        return base.Controls;
      }
    }

    protected internal new EventHandlerList Events
    {
      get { return base.Events; }
    }

    public class EmptyTestServer : HttpServerUtilityBase
    {

      public override int ScriptTimeout { get; set; }

    }

    public class EmptyHttpContext : HttpContextBase {  }


  }

}
