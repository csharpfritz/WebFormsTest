using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace WebFormsTest
{
  public class TestablePage : Page
  {

    public const string COMMENT_MARKER = "<!-- From a testable web page -->";
    private HttpContextBase _Context;

    public enum WebFormEvent
    {
      Init,
      Load,
      PreRender,
      Unload
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

  }

}
