using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fritz.WebFormsTest.Web.Scenarios.ControlTree
{
  public partial class Buttons_AutoId : TestablePage
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      mainForm.DataSource = new object[] { };
      mainForm.DataBind();
    }

    public Button ButtonA
    {
      get { return mainForm.FindControl("buttonA") as Button; }
    }

    public Button ButtonB
    {
      get { return mainForm.FindControl("buttonB") as Button; }
    }


    protected void buttonA_Click(object sender, EventArgs e)
    {
      ButtonA.Enabled = false;
      ButtonB.Enabled = true;
    }

    protected void buttonB_Click(object sender, EventArgs e)
    {
      ButtonA.Enabled = true;
      ButtonB.Enabled = false;
    }

    protected void buttonA_Command(object sender, CommandEventArgs e)
    {

      if (e.CommandName == "Caption")
        ButtonA.Text = e.CommandArgument.ToString();

    }
  }
}