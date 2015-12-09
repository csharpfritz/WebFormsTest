using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace WebFormsInspect.Base
{
  public class BaseWebForm : Page
  {

    private HttpContextBase _Context;

    //public BaseWebForm() : this(new HttpContextWrapper(HttpContext.Current)) { }
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

    public new HttpResponseBase Response
    {
      get { return Context.Response; }
    }

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

    protected override void OnPreRender(EventArgs e)
    {

      base.OnPreRender(e);

      this.Response.Write("<!-- FROM THE BASE PAGE -->");

    }

  }
}