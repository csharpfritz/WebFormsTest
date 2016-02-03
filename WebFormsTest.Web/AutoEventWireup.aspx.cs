using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fritz.WebFormsTest.Web
{
  public partial class AutoEventWireup : Page
  {

    public const string LOAD_INDICATOR = "<!-- Page Loaded -->";

    protected void Page_Load(object sender, EventArgs e)
    {

      // Write content to prove that the Load event triggered
      Controls.Add(new LiteralControl(LOAD_INDICATOR));
      // Response.Write(LOAD_INDICATOR);

    }

  }
}