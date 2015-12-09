using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebFormsInspect
{
  public partial class _Default : Base.BaseWebForm
  {

    public const string LOAD_INDICATOR = "<!-- LOAD WAS CALLED -->";

    public _Default()
    {
      this.Load += Page_Load;
    }

    protected void Page_Load(object sender, EventArgs e)
    {

      Response.Write(LOAD_INDICATOR);

    }
  }
}