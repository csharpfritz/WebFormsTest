using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fritz.WebFormsTest.Web.UserControls
{
  public partial class MyLinkControl : System.Web.UI.UserControl
  {
    protected void Page_Load(object sender, EventArgs e)
    {

      Response.Write($"<!-- My context contains {Context.Items.Count} items -->");

    }


    public string LinkButtonText
    {
      get { return myLink.Text; }
      set { myLink.Text = value; }
    }

  }
}