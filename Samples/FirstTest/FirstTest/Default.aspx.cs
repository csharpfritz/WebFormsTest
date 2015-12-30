using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FirstTest
{
  public partial class Default : WebFormsTest.TestablePage
  {

    public Default()
    {
      Load += Page_Load;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
      LoadEventTriggered = true;
    }

    public bool LoadEventTriggered = false;

  }
}