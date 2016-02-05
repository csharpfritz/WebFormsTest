using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Fritz.WebFormsTest
{

  public static class HttpContextExtensions
  {

    public static void AddSession(this HttpContext ctx, HttpSessionState session)
    {

      ctx.Items["AspSession"] = session;

    }

  }

}
