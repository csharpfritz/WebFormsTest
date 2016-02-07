using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Compilation;

namespace Fritz.WebFormsTest.Internal
{

  public class BuildManagerWrapper
  {

    private static readonly Type _Type = typeof(BuildManager);

    public static BuildManager TheBuildManager
    {
      get
      {
        var p =_Type.GetProperty("TheBuildManager", BindingFlags.Static | BindingFlags.NonPublic);
        return p.GetValue(null, null) as BuildManager;
      }
    }

    public static bool TopLevelFilesCompiledCompleted
    {

      get
      {

        var f = _Type.GetField("_topLevelFilesCompiledCompleted", BindingFlags.Instance | BindingFlags.NonPublic);
        return (bool)f.GetValue(TheBuildManager);

      }

    }

    public static string GenerateRandomAssemblyName(string baseName, bool topLevel)
    {

      var method = _Type.GetMethod("GenerateRandomAssemblyName", BindingFlags.NonPublic | BindingFlags.Static);
      return method.Invoke(null, new object[] { baseName, topLevel }).ToString();

    }

    public static object MemoryCache
    {
      get
      {
        var fld = _Type.GetField("_memoryCache", BindingFlags.Instance | BindingFlags.NonPublic);
        return fld.GetValue(TheBuildManager);
      }
    }

    public static void RegularAppRuntimeModeInitialize()
    {

      var initMethod = _Type.GetMethod("RegularAppRuntimeModeInitialize", BindingFlags.Instance | BindingFlags.NonPublic);
      initMethod.Invoke(TheBuildManager, null);

    }

  }

}
