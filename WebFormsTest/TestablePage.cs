using System;
using System.ComponentModel;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

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

    private TestServer _TestServer;

    /// <summary>
    /// The events from a Page that can be triggered within a TestablePage
    /// </summary>
    public enum WebFormEvent
    {
      None = 0,
      Init,
      Load,
      PreRender,
      Unload
    }

    private readonly Dictionary<WebFormEvent, bool> _EventsTriggered = new Dictionary<WebFormEvent, bool>();

    public TestablePage()
    {

      if (IsInTestMode)
      {
        // Reference the empty context here in case any nasty framework items are looking for it before a mock is provided
        Context = new EmptyHttpContext();
      }

    }

    /// <summary>
    /// Prepare the page for testing by triggering all of the setup methods that a Page would normally have triggered when running on a web server
    /// </summary>
    public void PrepareTests()
    {

      // This is a test-only concern
      if (!IsInTestMode) return;

      // NOTE: This is a COMPLETE fake out and wrap around the generated code
      base.FrameworkInitialize();
      var mi = this.GetType().GetMethod("__BuildControlTree", BindingFlags.NonPublic | BindingFlags.Instance);
      mi.Invoke(this, new object[] { this });

      HookupAutomaticHandlersInTest();

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

    public void FireEvent(WebFormEvent e)
    {
      FireEvent(e, EventArgs.Empty);
    }

    public void FireEvent(WebFormEvent e, EventArgs args)
    {

      if (_EventsTriggered.ContainsKey(e))
      {
        throw new InvalidOperationException($"Previously triggered the {e.ToString()} event");
      }

      _EventsTriggered.Add(e, true);

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
      get { return (HttpContext.Current == null || 
          (HttpContext.Current.Items.Contains("IsInTestMode") && (bool)(HttpContext.Current.Items["IsInTestMode"])  )); }
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
      EnsureChildControls();

      // Set the local fields that the postData SHOULD have come from
      this.Set_RequestValueCollection(postData);
      this.Set_UnvalidatedRequestValueCollection(postData);

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

    public void RunToEvent(WebFormEvent evt = WebFormEvent.None)
    {

      FireEvent(WebFormEvent.Init);
      if (evt == WebFormEvent.Init) return;

      FireEvent(WebFormEvent.Load);
      if (evt == WebFormEvent.Load) return;

      FireEvent(WebFormEvent.PreRender);
      if (evt == WebFormEvent.PreRender) return;

      FireEvent(WebFormEvent.Unload);


    }


    public TestablePage SetPageState<T>(string controlId, Action<T> controlConfig) where T : Control
    {

      // Prevent this method from being called by non-test operations
      if (!IsInTestMode) throw new InvalidOperationException("This method should only be run in Test mode");
      if (!WebApplicationProxy.IsInitialized) throw new InvalidOperationException("A WebApplicationProxy is needed to set page state");

      var c = this.FindControl(controlId);

      if (c == null) throw new ArgumentException($"Unable to locate the control '{controlId}'");
      if (!(c is T)) throw new ArgumentException($"The control '{controlId}' is not of type '{typeof(T).FullName}'");

      controlConfig(c as T);

      return this;

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
          if (_TestServer == null) _TestServer = new TestServer();
          return _TestServer;
        }

        return Context.Server;

      }
    }

    public override bool EnableEventValidation
    {
      get
      {

        // turn off event validation when in test mode

        return !IsInTestMode ? base.EnableEventValidation : false;
      }

      set
      {
        base.EnableEventValidation = value;
      }
    }

    #endregion

    #region Reflection Helpers to get to non-public root fields

    private void Set_RequestValueCollection(NameValueCollection newValue)
    {

      var field = typeof(Page).GetField("_requestValueCollection", BindingFlags.NonPublic | BindingFlags.Instance);
      field.SetValue(this, newValue);

    }

    private void Set_UnvalidatedRequestValueCollection(NameValueCollection newValue)
    {

      var field = typeof(Page).GetField("_unvalidatedRequestValueCollection", BindingFlags.NonPublic | BindingFlags.Instance);
      field.SetValue(this, newValue);

    }

    #endregion

    /// <summary>
    /// Simple handler to add a marker to the end of a page that shows that this is a TestablePage when debugging is enabled
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreRender(EventArgs e)
    {

      base.OnPreRender(e);

      if (Context.IsDebuggingEnabled)
      {
        this.Controls.Add(new LiteralControl(COMMENT_MARKER));
      }

    }

    #region Embedded helper classes

    public class EmptyHttpContext : HttpContextBase
    {

      public static explicit operator HttpContext(EmptyHttpContext v)
      {
        // do something...
        return new HttpContext(null, null);
      }

    }

    #endregion


    private MasterPage _master;
    private bool _preInitWorkComplete = false;
    public new MasterPage Master
    {
      get
      {

        if (!IsInTestMode) return base.Master;

        CreateMasterInTest();

        return _master;

      }
    }

    /// <summary>
    /// If there is a MasterPageFile defined, load the MasterPage and process it for this Page
    /// </summary>
    private void CreateMasterInTest()
    {

      if (MasterPageFile == null) return;

      _master = WebApplicationProxy.GetPageByLocation(MasterPageFile) as MasterPage;
      WebApplicationProxy.SubstituteDummyHttpContext();

      if (HasControls()) Controls.Clear();

      var contentTemplates = ContentTemplateCollection;
      _master.SetContentTemplates(contentTemplates);
      _master.SetOwnerControl(this);


      Debug.Assert(HttpContext.Current != null, "HttpContext.Current is missing!");

      _master.InitializeAsUserControl(this.Page);
      this.Controls.Add(_master);

    }

    internal object MasterPageFileInternal
    {
      get
      {

        var f = typeof(Page).GetField("_masterPageFile", BindingFlags.NonPublic | BindingFlags.Instance);
        return f.GetValue(this);

      }
    }

    private IDictionary ContentTemplateCollection
    {
      get
      {

        var p = typeof(Page).GetField("_contentTemplateCollection", BindingFlags.NonPublic | BindingFlags.Instance);
        return p.GetValue(this) as IDictionary;

      }
    }

    /// <summary>
    /// Trigger the assignment of event handlers to events if they match the prescribed naming convention
    /// </summary>
    private void HookupAutomaticHandlersInTest()
    {

      if (!IsInTestMode) return;

      var mi = typeof(TemplateControl).GetMethod("HookUpAutomaticHandlers", BindingFlags.NonPublic | BindingFlags.Instance);
      mi.Invoke(this, null);

    }

  }

}
