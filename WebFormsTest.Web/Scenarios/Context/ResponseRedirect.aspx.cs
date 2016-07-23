using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fritz.WebFormsTest.Web.Scenarios.Context
{

    /// <summary>
    /// Provide a page that forces a redirect so that we can inspect that the 
    /// test harness allows this to be inspected
    /// </summary>
    public partial class ResponseRedirect : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("/default.aspx");
        }
    }
}