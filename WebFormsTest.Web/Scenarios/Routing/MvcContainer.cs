using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Fritz.WebFormsTest.Web.Scenarios.Routing
{
  public class MvcContainer
  {
    private Controller _controller;
    private ViewPage _viewPage;

    public class WebFormsController : Controller { }

    public HtmlHelper Html { get; private set; }
    public UrlHelper Url { get; private set; }
    public dynamic ViewBag { get; private set; }

    public MvcContainer(HttpContext context)
    {
      this._controller = new WebFormsController();
      this._viewPage = new ViewPage();
      InitializeHtmlHelper(context);
    }

    private void InitializeHtmlHelper(HttpContext context)
    {
      var httpContext = new HttpContextWrapper(context);
      var controllerContext = new ControllerContext(httpContext, new RouteData(), _controller);
      var viewContext = new ViewContext(controllerContext, new WebFormView(controllerContext, "Views"), new ViewDataDictionary(), new TempDataDictionary(), TextWriter.Null);
      Html = new HtmlHelper(viewContext, _viewPage);
      Url = new UrlHelper(new RequestContext(httpContext, RouteTable.Routes.GetRouteData(httpContext) ?? new RouteData()));
      ViewBag = viewContext.ViewBag;
    }

    public void Dispose()
    {
      _controller.Dispose();
      _viewPage.Dispose();
    }
  }
}