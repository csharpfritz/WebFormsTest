using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Reflection;

namespace WebFormsTest
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

    public enum WebFormEvent
    {
      Init,
      Load,
      PreRender,
      Unload
    }

    public TestablePage()
    {

      if (IsTestingEnabled)
      {
        this.AutoEventWireup();
      }

    }

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
    public bool IsTestingEnabled
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
      EnsureChildControls();

      // Load the data
      var hiddenMethod = typeof(Page).GetMethod("ProcessPostData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      hiddenMethod.Invoke(this, new object[] { postData, true });

    }

    /// <summary>
    /// Check if there are event handlers configured in the page for the standard events and connect them
    /// </summary>
    private void AutoEventWireup()
    {

      RegisterEventHandlerIfMissing("Load");

    }

    private void RegisterEventHandlerIfMissing(string name)
    {

      /// Attempt at doing this with Reflection...  running into a security error

      var methodName = $"Page_{name}";
      if (!IsMethodPresent(methodName)) return;

      // Walk the internal structure of the Event to get the Event Delegate
      var thisEvent = GetType().GetEvent(name, AllBindings);
      Type tEvent = thisEvent.EventHandlerType;

      var eventHandlerList = thisEvent.DeclaringType.GetField("Events", AllBindings);

      MethodInfo mi = GetType().GetMethod(methodName, AllBindings);

      /**
            Delegate d = Delegate.CreateDelegate(tEvent, mi);

            // Remove it first, then re-add it
            thisEvent.RemoveEventHandler(this, d);
            thisEvent.AddEventHandler(this, d);
        **/
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

    #endregion

    protected override void OnPreRender(EventArgs e)
    {

      base.OnPreRender(e);

      if (Context.IsDebuggingEnabled)
      {
        this.Controls.Add(new LiteralControl(COMMENT_MARKER));
      }

    }

    protected internal new EventHandlerList Events
    {
      get { return base.Events; }
    }

  }

}
