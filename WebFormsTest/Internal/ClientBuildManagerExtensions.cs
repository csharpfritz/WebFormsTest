using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Compilation;

namespace Fritz.WebFormsTest.Internal
{

  internal static class ClientBuildManagerExtensions
  {

    public static object GetAppId(this ClientBuildManager compiler)
    {

      return compiler.GetType()
        .GetField("_appId", BindingFlags.NonPublic | BindingFlags.Instance)
        .GetValue(compiler);


    }

  }
}
