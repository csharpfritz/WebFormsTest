using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fritz.WebFormsTest.Web
{
  public partial class _Default : Page
  {

    public const string LOAD_INDICATOR = "<!-- LOAD WAS CALLED -->";

    public _Default()
    {
      Page.Load += Page_Load;
    }

    protected void Page_Load(object sender, EventArgs e)
    {

      if (Request.Form != null && Request.Form.Count > 0)
      {
        Response.Write(Request.Form["test"]);
      }

      Response.Write(LOAD_INDICATOR);

      var getRunTime = typeof(HttpRuntime).GetField("_theRuntime", BindingFlags.NonPublic | BindingFlags.Static);
      var theRunTime = getRunTime.GetValue(null) as HttpRuntime;

      var p = typeof(HttpRuntime).GetField("_appDomainAppVPath", BindingFlags.NonPublic | BindingFlags.Instance);
      var outValue = p.GetValue(theRunTime);

      Response.Write("_appDomainAppVPath: " + outValue);


    }

    protected override void OnPreRender(EventArgs e)
    {

      base.OnPreRender(e);

      this.Controls.Add(new LiteralControl("<!-- Default Page -->"));

    }

  }
}