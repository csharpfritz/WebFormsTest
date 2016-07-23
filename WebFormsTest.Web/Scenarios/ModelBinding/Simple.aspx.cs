using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fritz.WebFormsTest.Web.Scenarios.ModelBinding
{
    public partial class Simple : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        public IEnumerable<GridItem> Get()
        {
            return new GridItem[] {
                new GridItem {ID=1, Name="Test" },
                new GridItem {ID=2, Name="Foo" }
            };
        }

    }

    public class GridItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}