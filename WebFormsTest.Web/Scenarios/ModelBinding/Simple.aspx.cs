using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fritz.WebFormsTest.Web.Scenarios.ModelBinding
{
    public partial class Simple : System.Web.UI.Page
    {

        public static readonly GridItem[] SampleItems = new GridItem[] {
                new GridItem {ID=1, Name="Test" },
                new GridItem {ID=2, Name="Foo" }
            };

        protected void Page_Load(object sender, EventArgs e)
        {
            Comment = new LiteralControl();
            Page.Controls.Add(Comment);
        }

        internal LiteralControl Comment
        {
            get; set;
        }

        public IEnumerable<GridItem> Get()
        {
            return SampleItems;
        }

    }

  public class GridItem
  {
    public int ID { get; set; }

    [Required, MinLength(3)]
    public string Name { get; set; }
  }

}