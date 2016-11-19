using Microsoft.AspNet.FriendlyUrls.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Fritz.WebFormsTest.Web.Scenarios.ModelBinding.Simple;

namespace Fritz.WebFormsTest.Web.Scenarios.ModelBinding
{
    public partial class EditForm : System.Web.UI.Page
    {

        public static readonly List<GridItem> SampleItems = new List<GridItem> {
                new GridItem {ID=1, Name="Test" },
                new GridItem {ID=2, Name="Foo" }
            };


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public FormView TheForm {  get { return myForm; } }
        

        // The id parameter should match the DataKeyNames value set on the control
        // or be decorated with a value provider attribute, e.g. [QueryString]int id
        public GridItem myForm_GetItem([FriendlyUrlSegments(0)]int? id)
        {
            return SampleItems.FirstOrDefault(i => i.ID == id);
        }

        // The id parameter name should match the DataKeyNames value set on the control
        public void myForm_UpdateItem(int id)
        {
            GridItem item = SampleItems.FirstOrDefault(i => i.ID == id);
            if (item == null)
            {
                // The item wasn't found
                ModelState.AddModelError("", String.Format("Item with id {0} was not found", id));
                return;
            }
            TryUpdateModel(item);
            if (ModelState.IsValid)
            {
                // Save changes here, e.g. MyDataLayer.SaveChanges();
                SampleItems.Remove(SampleItems.First(i => i.ID == id));
                SampleItems.Add(item);
                myForm.DefaultMode = FormViewMode.ReadOnly;
            }
        }

        public void myForm_InsertItem()
        {
            var item = new GridItem();
            TryUpdateModel(item);
            if (ModelState.IsValid)
            {
                // Save changes here

            }
        }
    }
}