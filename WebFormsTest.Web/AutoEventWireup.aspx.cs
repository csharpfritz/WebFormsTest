using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fritz.WebFormsTest.Web
{
  public partial class AutoEventWireup : TestablePage
  {

    public const string LOAD_INDICATOR = "<!-- Page Loaded -->";

    protected void Page_Load(object sender, EventArgs e)
    {

      // Write content to prove that the Load event triggered
      Response.Write(LOAD_INDICATOR);

    }

  }
}