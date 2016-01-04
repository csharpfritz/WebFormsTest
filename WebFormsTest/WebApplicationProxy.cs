using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web;
using System.Web.Compilation;

namespace Fritz.WebFormsTest
{

  /// <summary>
  /// A class that wraps a web application project and provides access to the .NET code contained within
  /// </summary>
  public class WebApplicationProxy : IDisposable
  {

    private static WebApplicationProxy _Instance;
    private string _WebRootFolder;
    private readonly Dictionary<Type, string> _LocationTypeMap = new Dictionary<Type, string>();
    private ClientBuildManager _compiler;
    private string _TargetFolder;
    private bool _SkipCrawl = false;
    private bool Initialized = false;


    /// <summary>
    /// Create a proxy for the web application to be inspected
    /// </summary>
    /// <param name="rootFolder">Physical location on disk of the web application source code</param>
    private WebApplicationProxy(string rootFolder, bool skipCrawl)
    {

      _WebRootFolder = rootFolder;
      _SkipCrawl = skipCrawl;

    }

    ~WebApplicationProxy()
    {
      Dispose(false);
    }

    public static void Create(string rootFolder, bool skipCrawl = true)
    {
      _Instance = new WebApplicationProxy(rootFolder, skipCrawl);
    }

    /// <summary>
    /// The folder that the Web Application resides in
    /// </summary>
    public static string WebApplicationRootFolder {  get { return _Instance._TargetFolder; } }

    /// <summary>
    /// Inspect the application and create mappings for all types
    /// </summary>
    private void InitializeInternal()
    {

      if (Initialized) return;

      _TargetFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

      _compiler = new ClientBuildManager(@"/",
        _WebRootFolder,
        _TargetFolder,
        new ClientBuildManagerParameter() { PrecompilationFlags = PrecompilationFlags.ForceDebug });
      _compiler.PrecompileApplication();

      CrawlWebApplication();

      Initialized = true;

    }

    public static void Initialize()
    {

      InjectTestValuesIntoHttpRuntime();
      SubstituteDummyHttpContext();

      _Instance.InitializeInternal();
    }

    /// <summary>
    /// Add some needed configuration to the HttpRuntime so that it thinks we are running in a Web Service
    /// </summary>
    private static void InjectTestValuesIntoHttpRuntime()
    {

      var getRunTime = typeof(HttpRuntime).GetField("_theRuntime", BindingFlags.NonPublic | BindingFlags.Static);
      var theRunTime = getRunTime.GetValue(null) as HttpRuntime;

      var p = typeof(HttpRuntime).GetField("_appDomainAppVPath", BindingFlags.NonPublic | BindingFlags.Instance);
      p.SetValue(theRunTime, CreateVirtualPath("/"));

    }

    /// <summary>
    /// Inspect the source code of the application, gathering the page locations and the types at those locations
    /// </summary>
    private void CrawlWebApplication()
    {

      if (_SkipCrawl) return;

      // TODO: Crawl the web application and collect the ASPX / Type mappings from the Page directives

      throw new NotImplementedException("Not yet implemented web application source crawl");

    }

    /// <summary>
    /// Get a fully instantiated Page at the web location specified
    /// </summary>
    /// <param name="location">The web absolute folder location to retrive the Page from</param>
    /// <returns>The Page object from the specified location</returns>
    public static object GetPageByLocation(string location)
    {

      if (_Instance == null || !_Instance.Initialized) throw new InvalidOperationException("The WebApplicationProxy has not been created and initialized properly");

      var returnType = _Instance._compiler.GetCompiledType(location);
      SubstituteDummyHttpContext();

      return Activator.CreateInstance(returnType);

    }

    /// <summary>
    /// Get a fully instantiated Page at the web location specified
    /// </summary>
    /// <typeparam name="T">The type of the Page to fetch</typeparam>
    /// <param name="location">The web absolute folder location to retrive the Page from</param>
    /// <returns>A strongly-typed Page object from the specified location</returns>
    public static T GetPageByLocation<T>(string location) where T : TestablePage
    {

      return GetPageByLocation(location) as T;

    }

    public static T GetPageByType<T>() where T : TestablePage
    {

      if (_Instance._SkipCrawl) throw new InvalidOperationException("Unable to fetch by type because the project has not been indexed");

      var locn = _Instance._LocationTypeMap[typeof(T)];

      return GetPageByLocation<T>(locn);

    }

    public static object GetPageByType(Type pageType)
    {

      if (_Instance._SkipCrawl) throw new InvalidOperationException("Unable to fetch by type because the project has not been indexed");

      var locn = _Instance._LocationTypeMap[pageType];

      return GetPageByLocation(locn);

    }

    public static void DisposeIt()
    {
      _Instance.Dispose();
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

    /// <summary>
    /// Swap in a dummy HttpContext for HttpContext.Current, valid for current thread only
    /// </summary>
    internal static void SubstituteDummyHttpContext()
    {

      HttpContext.Current = GetDummyContext("/");

    }

    /// <summary>
    /// Get a simple and DUMB HttpContext suitable for replacing HttpContext.Current for a page requested at <paramref name="path"/>
    /// </summary>
    /// <param name="path">The location of the request to fake</param>
    /// <returns></returns>
    internal static HttpContext GetDummyContext(string path)
    {

      var request = GetSimpleHttpRequest(path);

      var sw = new StringWriter();
      var response = new HttpResponse(sw);

      var myContext = new HttpContext(request, response);
      myContext.Items["IsInTestMode"] = true;

      return myContext;

    }

    /// <summary>
    /// Create a simple HttpRequest to the location specified in <paramref name="path"/>
    /// </summary>
    /// <param name="path">Location requested</param>
    /// <returns></returns>
    private static HttpRequest GetSimpleHttpRequest(string path)
    {

      var vpType = GetVirtualPathType();
      var ctor = typeof(HttpRequest).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { vpType, typeof(String) }, null);

      return ctor.Invoke(new[] { CreateVirtualPath(path), string.Empty }) as HttpRequest;

    }

    #region Helper methods to access the internal System.Web.VirtualPath type

    private static object CreateVirtualPath(string path)
    {
      Type vpType = GetVirtualPathType();

      var mi = vpType.GetMethod("Create", new[] { typeof(string) });

      var outObj = mi.Invoke(null, new object[] { path });

      return outObj;

    }

    private static Type GetVirtualPathType()
    {
      var a = typeof(System.Web.UI.Page).Assembly;
      var vpType = a.GetType("System.Web.VirtualPath");
      return vpType;
    }

    #endregion

  }

}
