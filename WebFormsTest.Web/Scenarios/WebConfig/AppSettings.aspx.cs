using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fritz.WebFormsTest.Web.Scenarios.WebConfig
{
  public partial class AppSettings : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      this.testSetting.Text = TestConfigValue;
      this.testInitialCatalog.Text = TestInitialCatalog;
    }

    public string TestConfigValue
    {
      get
      {
        return ConfigurationManager.AppSettings["testSetting"];
      }
    }

    public string TestInitialCatalog
    {
      get
      {
        var connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        var elements = connString.Split(';');
        var initialCatalog = elements.First(el => el.Contains("Initial Catalog"));
        return initialCatalog.Split('=')[1];
      }
    }

  }
}