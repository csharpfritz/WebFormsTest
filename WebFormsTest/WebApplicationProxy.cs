using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Compilation;

namespace Fritz.WebFormsTest
{

  /// <summary>
  /// A class that wraps a web application project and provides access to the .NET code contained within
  /// </summary>
  public class WebApplicationProxy : IDisposable
  {

    private string _WebRootFolder;
    private readonly Dictionary<Type, string> _LocationTypeMap = new Dictionary<Type, string>();
    private ClientBuildManager _compiler;
    private string _TargetFolder;
    private bool _SkipCrawl = false;


    /// <summary>
    /// Create a proxy for the web application to be inspected
    /// </summary>
    /// <param name="rootFolder">Physical location on disk of the web application source code</param>
    public WebApplicationProxy(string rootFolder, bool skipCrawl)
    {

      _WebRootFolder = rootFolder;
      _SkipCrawl = skipCrawl;

    }

    ~WebApplicationProxy()
    {
      Dispose(false);
    }

    /// <summary>
    /// The folder that the Web Application resides in
    /// </summary>
    public string WebApplicationRootFolder {  get { return _TargetFolder; } }

    /// <summary>
    /// Inspect the application and create mappings for all types
    /// </summary>
    public void Initialize()
    {

      _TargetFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

      _compiler = new ClientBuildManager(@"/",
        _WebRootFolder,
        _TargetFolder,
        new ClientBuildManagerParameter() { PrecompilationFlags = PrecompilationFlags.ForceDebug });

      CrawlWebApplication();

    }

    private void CrawlWebApplication()
    {

      if (_SkipCrawl) return;

    }

    public object GetPageByLocation(string location)
    {

      var returnType = _compiler.GetCompiledType(location);

      return Activator.CreateInstance(returnType);

    }

    public T GetPageByLocation<T>(string location) where T : TestablePage
    {

      var returnType = _compiler.GetCompiledType(location);

      return Activator.CreateInstance(returnType) as T;
    }

    public T GetPageByType<T>() where T : TestablePage
    {

      if (_SkipCrawl) throw new InvalidOperationException("Unable to fetch by type because the project has not been indexed");

      var locn = _LocationTypeMap[typeof(T)];

      return GetPageByLocation<T>(locn);

    }

    public object GetPageByType(Type pageType)
    {

      if (_SkipCrawl) throw new InvalidOperationException("Unable to fetch by type because the project has not been indexed");

      var locn = _LocationTypeMap[pageType];

      return GetPageByLocation(locn);

    }

    public void Dispose()
    {
      Dispose(true);
    }

    protected virtual void Dispose(bool isDisposing)
    {

      if (isDisposing) GC.SuppressFinalize(this);

      // Clean up the target folder
      try {
        Directory.Delete(WebApplicationRootFolder, true);
      } catch (DirectoryNotFoundException)
      {
        // Its ok...  its already gone!
      }
    }

  }

}
