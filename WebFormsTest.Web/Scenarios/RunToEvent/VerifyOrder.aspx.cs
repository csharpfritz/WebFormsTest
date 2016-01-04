using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fritz.WebFormsTest.Web.Scenarios.RunToEvent
{
  public partial class VerifyOrder : TestablePage
  {

    protected override void OnInit(EventArgs e)
    {
      base.OnInit(e);
      EventList.Add(1,"1 - Init");
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      EventList.Add(2,"2 - Load");
    }

    protected override void OnPreRender(EventArgs e)
    {
      base.OnPreRender(e);
      EventList.Add(3,"3 - PreRender");
    }

    protected override void OnUnload(EventArgs e)
    {
      base.OnUnload(e);
      EventList.Add(4,"4 - Unload");
    }


    public SortedList<int,string> EventList = new SortedList<int,string>();

  }
}