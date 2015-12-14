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
    }

    protected void Page_Load(object sender, EventArgs e)
    {

      Response.Write(LOAD_INDICATOR);

    }
  }
}