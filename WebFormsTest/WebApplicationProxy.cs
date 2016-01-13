using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Compilation;

namespace Fritz.WebFormsTest
{

  /// <summary>
  /// A class that wraps a web application project and provides access to the .NET code contained within
  /// </summary>
  public class WebApplicationProxy : IDisposable
  {

    private static WebApplicationProxy _Instance;
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

      WebRootFolder = rootFolder;
      _SkipCrawl = skipCrawl;

    }

    ~WebApplicationProxy()
    {
      Dispose(false);
    }

    public static void Create(string rootFolder, bool skipCrawl = true)
    {
      _Instance = new WebApplicationProxy(rootFolder, skipCrawl);

      InjectTestValuesIntoHttpRuntime();
      SubstituteDummyHttpContext();

      _Instance.InitializeInternal();

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

      // Can this go async?
      _compiler = new ClientBuildManager(@"/",
        WebRootFolder,
        _TargetFolder,
        new ClientBuildManagerParameter() { PrecompilationFlags = PrecompilationFlags.ForceDebug });
      _compiler.PrecompileApplication();


      // Async?
      CrawlWebApplication();

      Type appType = GetHttpApplicationType();
      WebApplicationProxy.Application = Activator.CreateInstance(appType) as HttpApplication;
      TriggerApplicationStart(WebApplicationProxy.Application);

      Initialized = true;

    }

    [Obsolete("Now initializing during Create")]
    public static void Initialize() {  }

    /// <summary>
    /// Check if the Proxy is running
    /// </summary>
    public static bool IsInitialized {  get { return _Instance != null ? _Instance.Initialized : false; } }

    /// <summary>
    /// Add some needed configuration to the HttpRuntime so that it thinks we are running in a Web Service
    /// </summary>
    private static void InjectTestValuesIntoHttpRuntime()
    {

      var getRunTime = typeof(HttpRuntime).GetField("_theRuntime", BindingFlags.NonPublic | BindingFlags.Static);
      var theRunTime = getRunTime.GetValue(null) as HttpRuntime;

      var p = typeof(HttpRuntime).GetField("_appDomainAppVPath", BindingFlags.NonPublic | BindingFlags.Instance);
      p.SetValue(theRunTime, VirtualPathWrapper.Create("/").VirtualPath);

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
    public static object GetPageByLocation(string location, HttpContextBase context = null)
    {

      if (_Instance == null || !_Instance.Initialized) throw new InvalidOperationException("The WebApplicationProxy has not been created and initialized properly");

      var returnType = _Instance._compiler.GetCompiledType(location);
      SubstituteDummyHttpContext();

      dynamic outObj = Activator.CreateInstance(returnType);

      if (context != null)
      {
        var pi = outObj.GetType().GetProperty("Context");
        pi.SetValue(outObj, context, null);
      }

      // Prepare the page for testing
      if (outObj is TestablePage) outObj.PrepareForTest();

      return outObj;


    }

    /// <summary>
    /// Get a fully instantiated Page at the web location specified
    /// </summary>
    /// <typeparam name="T">The type of the Page to fetch</typeparam>
    /// <param name="location">The web absolute folder location to retrive the Page from</param>
    /// <returns>A strongly-typed Page object from the specified location</returns>
    public static T GetPageByLocation<T>(string location, HttpContextBase context = null) where T : TestablePage
    {

      return GetPageByLocation(location, context) as T;

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

    public static HttpApplication Application
    {
      get; private set;
    }

    public static Cache Cache
    {
      get
      {

        if (HttpContext.Current == null) SubstituteDummyHttpContext();
        return HttpContext.Current.Cache;

      }
    }

    public static string WebRootFolder
    {
      get; private set;
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
    internal static HttpRequest GetSimpleHttpRequest(string path)
    {

      var ctor = typeof(HttpRequest).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { VirtualPathWrapper.VirtualPathType, typeof(String) }, null);

      return ctor.Invoke(new object[] { VirtualPathWrapper.Create(path).VirtualPath, string.Empty }) as HttpRequest;

    }

    private static VirtualPathWrapper _BaseVirtualPath;
    /// <summary>
    /// A VirtualPath reference to the root of the application
    /// </summary>
    internal static VirtualPathWrapper BaseVirtualPath
    {
      get
      {
        if (_BaseVirtualPath == null)
        {
          _BaseVirtualPath = VirtualPathWrapper.CreateAbsolute(WebRootFolder);
        }
        return _BaseVirtualPath;
      }
    }

    #region Private Methods

    /// <summary>
    /// Get the HttpApplication Type (typically named Global) from the application
    /// </summary>
    /// <returns></returns>
    internal static Type GetHttpApplicationType()
    {

      // Force the web application to be loaded 
      _Instance._compiler.GetCompiledType("/Default.aspx");

      // Search the AppDomain of loaded types that inherit from HttpApplication, starting with "Global"
      Type httpApp = typeof(HttpApplication);

      Type outType;

      // Blacklist assembly name prefixes
      var blacklist = new string[] { "Microsoft.", "System.", "mscorlib", "xunit." };

      var assembliesToScan = AppDomain.CurrentDomain.GetAssemblies().Where(a => !blacklist.Any(b => a.FullName.StartsWith(b))).ToArray();
      var theAssembly = assembliesToScan.FirstOrDefault(a =>
      {
        bool outValue = false;
        try
        {
          outValue = a.GetTypes().FirstOrDefault(t => t.FullName.EndsWith(".Global") && (t.BaseType == httpApp)) != null;
        }
        catch
        {
          outValue = false;
        }
        return outValue;
      });
      outType = theAssembly.GetTypes().First(t => t.BaseType == httpApp);

      return outType;

    }


    private static void TriggerApplicationStart(HttpApplication app)
    {

      var mi = app.GetType().GetMethod("Application_Start", BindingFlags.Instance | BindingFlags.NonPublic);
      mi.Invoke(app, new object[] { app, EventArgs.Empty });

    }

    #endregion

  }

   

}
