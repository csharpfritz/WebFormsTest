using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Compilation;

namespace Fritz.WebFormsTest
{

  /// <summary>
  /// A helper class that prepares the test suite for execution by compiling the ASPX / ASCX / App_Code contents appropriately
  /// </summary>
  public class WebFormsPrecompiler : IDisposable
  {

    private Assembly _TargetAssembly;

    public WebFormsPrecompiler(string webApplicationRootFolder, Assembly webFormsApplicationAssembly)
    {
      this.WebApplicationRootFolder = webApplicationRootFolder;
      this.TargetFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
      this._TargetAssembly = webFormsApplicationAssembly;

    }

    ~WebFormsPrecompiler()
    {

      // Clean up the compile target folder
      Dispose(false);

    }

    /// <summary>
    /// Root folder location of the web application project to precompile
    /// </summary>
    public string WebApplicationRootFolder { get; private set; }

    /// <summary>
    /// Target folder where application will be compiled to
    /// </summary>
    public string TargetFolder { get; private set; }

    public void Execute()
    {

      var c = new System.Web.Compilation.ClientBuildManager(@"/", WebApplicationRootFolder, TargetFolder);
      c.PrecompileApplication();

    }

    public Type CompilePage(string path)
    {

      var c = new System.Web.Compilation.ClientBuildManager(@"/", 
        WebApplicationRootFolder, 
        TargetFolder, 
        new System.Web.Compilation.ClientBuildManagerParameter() { PrecompilationFlags = System.Web.Compilation.PrecompilationFlags.ForceDebug });

      var returnType = c.GetCompiledType(path);

      return returnType;

    }

    public T GetPage<T>() where T : class
    {

      var c = new System.Web.Compilation.ClientBuildManager(@"/",
        WebApplicationRootFolder,
        TargetFolder,
        new System.Web.Compilation.ClientBuildManagerParameter()
        {
          PrecompilationFlags = PrecompilationFlags.ForceDebug |
                                PrecompilationFlags.CodeAnalysis |
                                PrecompilationFlags.Updatable
        });

      var returnObj = c.CreateObject(typeof(T), false);

      return returnObj as T;

    }


    public void Dispose()
    {
      Dispose(true);
    }

    protected void Dispose(bool isDisposing)
    {

      if (isDisposing) GC.SuppressFinalize(this);

      // Clean up the target folder
      Directory.Delete(TargetFolder, true);

    }



  }

}
