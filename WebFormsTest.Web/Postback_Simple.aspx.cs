using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebFormsTest;

namespace WebFormsInspect
{
  public partial class Postback_Simple : TestablePage
  {
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public TextBox Textbox { get { return this.textbox; } }

  }
}