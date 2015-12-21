using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebFormsTest;

namespace WebFormsTest
{
  public partial class _Default : TestablePage
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

    }
  }
}