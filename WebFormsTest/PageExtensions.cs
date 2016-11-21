using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Fritz.WebFormsTest
{

    public static class PageExtensions
    {

        private static readonly Type _Type = typeof(Page);

        /// <summary>
        /// Render the HTML for this page using the Render method and return the rendered string
        /// </summary>
        /// <returns></returns>
        public static string RenderHtml(this Page myPage)
        {

            var sb = new StringBuilder();
            var txt = new HtmlTextWriter(new StringWriter(sb));

            var renderMethod = _Type.GetMethod("Render", BindingFlags.Instance | BindingFlags.NonPublic);
            renderMethod.Invoke(myPage, new object[] { txt });

            return sb.ToString();

        }

        public static Page SetPageState<T>(this Page myPage, string controlId, Action<T> controlConfig) where T : Control
        {

            // Prevent this method from being called by non-test operations
            if (!WebApplicationProxy.IsInitialized) throw new InvalidOperationException("A WebApplicationProxy is needed to set page state");

            var c = myPage.FindControl(controlId);

            if (c == null) throw new ArgumentException($"Unable to locate the control '{controlId}'");
            if (!(c is T)) throw new ArgumentException($"The control '{controlId}' is not of type '{typeof(T).FullName}'");

            controlConfig(c as T);

            return myPage;

        }

        public static void MockPostData(this Page myPage, NameValueCollection postData)
        {

            // Ensure that the control structure is available
            var escMethod = _Type.GetMethod("EnsureChildControls", BindingFlags.Instance | BindingFlags.NonPublic);
            escMethod.Invoke(myPage, null);

            // Set the local fields that the postData SHOULD have come from
            var requestValueField = _Type.GetField("_requestValueCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            requestValueField.SetValue(myPage, postData);


            var collectionField = _Type.GetField("_unvalidatedRequestValueCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            collectionField.SetValue(myPage, postData);


            // Load the data
            var hiddenMethod = _Type.GetMethod("ProcessPostData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            hiddenMethod.Invoke(myPage, new object[] { postData, true });

        }

        public static Page AddToModelState(this Page myPage, string name, object value) {

           var modelStateProp = myPage.GetType().GetProperty("ModelState");
           ModelStateDictionary modelState = modelStateProp.Get

        }

        public static void FireEvent(this Page myPage, WebFormEvent e)
        {
            myPage.FireEvent(e, EventArgs.Empty);
        }

        public static void FireEvent(this Page myPage, WebFormEvent e, EventArgs args)
        {

            Dictionary<WebFormEvent, bool> _EventsTriggered = new Dictionary<WebFormEvent, bool>();
            if (myPage.Items.Contains("eventsTriggered"))
                _EventsTriggered = myPage.Items["eventsTriggered"] as Dictionary<WebFormEvent, bool>;

            if (_EventsTriggered.ContainsKey(e))
            {
                throw new InvalidOperationException($"Previously triggered the {e.ToString()} event");
            }

            _EventsTriggered.Add(e, true);
            myPage.Items["eventsTriggered"] = _EventsTriggered;

            string methodName = "";

            switch (e)
            {
                case WebFormEvent.Init:
                    //methodName = "OnInit";
                    break;
                case WebFormEvent.Load:
                    myPage.LoadRecursiveInternal();
                    break;
                case WebFormEvent.PreRender:
                    //methodName = "OnPreRender";
                    myPage.PreRenderRecursiveInternal();
                    break;
                case WebFormEvent.Unload:
                    methodName = "OnUnload";
                    break;
                default:
                    break;
            }

            if (methodName == string.Empty) return;

            var thisMethod = _Type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            thisMethod.Invoke(myPage, new object[] { args });

        }

        public static void RunToEvent(this Page myPage, WebFormEvent evt = WebFormEvent.None)
        {

            myPage.FireEvent(WebFormEvent.Init);
            if (evt == WebFormEvent.Init) return;

            myPage.FireEvent(WebFormEvent.Load);
            if (evt == WebFormEvent.Load) return;

            myPage.FireEvent(WebFormEvent.PreRender);
            if (evt == WebFormEvent.PreRender) return;

            myPage.FireEvent(WebFormEvent.Unload);

        }

        /// <summary>
        /// Helper property for inherited pages to indicate that we are in 'unit test mode'
        /// </summary>
        public static bool IsInTestMode(this Page myPage)
        {

            return (HttpContext.Current == null ||
              (HttpContext.Current.Items.Contains("IsInTestMode") && (bool)(HttpContext.Current.Items["IsInTestMode"])));

        }


        public static T FindControl<T>(this Page myPage, string controlId) where T : Control
        {

            return myPage.FindControl(controlId) as T;

        }

        internal static void PreRenderRecursiveInternal(this Page myPage)
        {

            var thisMethod = _Type.GetMethod("PreRenderRecursiveInternal", BindingFlags.Instance | BindingFlags.NonPublic);
            thisMethod.Invoke(myPage, new object[] { });

        }

        internal static void LoadRecursiveInternal(this Page myPage)
        {

            var thisMethod = _Type.GetMethod("LoadRecursive", BindingFlags.Instance | BindingFlags.NonPublic);
            thisMethod.Invoke(myPage, new object[] { });

        }

    }

}
