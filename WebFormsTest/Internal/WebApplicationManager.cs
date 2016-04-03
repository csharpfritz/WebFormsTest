using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;

namespace Fritz.WebFormsTest.Internal
{

  /// <summary>
  /// An internal resource that manages the application inside of the ShadowDomain
  /// </summary>
  internal class WebApplicationManager : MarshalByRefObject, IDisposable
  {

    private WebApplicationProxyOptions _Options;
    private bool _Initialized;
    private HostingEnvironmentWrapper _hostingEnvironment;
    private ClientBuildManager _compiler;
    private HttpApplication _WebApplication;

    public WebApplicationManager() { }

    public void Initialize(WebApplicationProxyOptions options)
    {

      _Options = options;

      AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", Path.Combine(options.PhysicalRootFolder, "web.config"));

      // Configure the HttpRuntime to understand that we are operating at
      // the root of our web application
      HttpRuntimeInstance
        .SetAppDomainVPath(VirtualPathWrapper.Create("/"));

      // Configure the hosting environment
      _hostingEnvironment = new HostingEnvironmentWrapper();

      // Mimic a first request that starts the JIT of the web application
      SubstituteDummyHttpContext("/");

      // NOTE: Can this part be pushed async?
      // Prepare the compiler
      _compiler = new ClientBuildManager(@"/",
        _Options.PhysicalRootFolder,
        null,
        new ClientBuildManagerParameter() {});

      // Configure the HttpRuntime
      HttpRuntimeInstance
        .SetAppDomainAppPath(_compiler.CodeGenDir)
        .SetAppId(_compiler.GetAppId())
        .SetPhysicalPath(options.PhysicalRootFolder);

      if (!options.SkipPrecompile) _compiler.PrecompileApplication();

      ConfigureBuildManager();

      // NOTE: Async?
      CrawlWebApplication();

      CreateHttpApplication();

      _Initialized = true;

    }

    public bool IsInitialized() { return _Initialized; }

    public object GetByLocation(string location, HttpBrowserCapabilities browserCaps, Action<HttpContext> contextModifiers = null)
    {

      Debug.WriteLine($"Domain: {AppDomain.CurrentDomain.FriendlyName}");

      var returnType = _compiler.GetCompiledType(location);
      SubstituteDummyHttpContext(location);

      if (contextModifiers != null) contextModifiers(HttpContext.Current);

      dynamic outObj = Activator.CreateInstance(returnType);

      // Prepare the page for testing
      if (outObj is Page) ((Page)outObj).PrepareForTest();

      CompleteHttpContext(HttpContext.Current);

      return outObj;

    }


    /// <summary>
    /// Swap in a dummy HttpContext for HttpContext.Current, valid for current thread only
    /// </summary>
    internal static void SubstituteDummyHttpContext(string location, HttpBrowserCapabilities caps = null)
    {

      var workerRequest = new TestHttpWorkerRequest(location);
      var testContext = new HttpContext(workerRequest);

      // BrowserCapabilities
      caps = caps ?? BrowserDefinitions.DEFAULT;
      testContext.Request.SetBrowserCaps(caps);

      testContext.Items["IsInTestMode"] = true;

      HttpContext.Current = testContext;

    }


    private static HttpRuntime HttpRuntimeInstance
    {
      get
      {
        var getRunTime = typeof(HttpRuntime).GetField("_theRuntime", BindingFlags.NonPublic | BindingFlags.Static);
        return getRunTime.GetValue(null) as HttpRuntime;
      }
    }

    /// <summary>
    /// Force the BuildManager into a state where it thinks we are on a WebHost and already precompiled
    /// </summary>
    private void ConfigureBuildManager()
    {

      var fld = typeof(BuildManager).GetProperty("PreStartInitStage", BindingFlags.Static | BindingFlags.NonPublic);
      var psiEnumValues = typeof(BuildManager).Assembly.GetType("System.Web.Compilation.PreStartInitStage").GetEnumValues();
      fld.SetValue(null, psiEnumValues.GetValue(2), null);

      // Skip Top Level Exceptions, just like we would in precompilation
      var skip = typeof(BuildManager).GetProperty("SkipTopLevelCompilationExceptions", BindingFlags.Static | BindingFlags.NonPublic);
      skip.SetValue(null, true, null);

      // RegularAppRuntimeModeInitialize
      BuildManagerWrapper.RegularAppRuntimeModeInitialize();

    }

    /// <summary>
    /// Inspect the source code of the application, gathering the page locations and the types at those locations
    /// </summary>
    private void CrawlWebApplication()
    {

      if (_Options.SkipCrawl) return;

      // TODO: Crawl the web application and collect the ASPX / Type mappings from the Page directives

      throw new NotImplementedException("Not yet implemented web application source crawl");

    }

    private void CreateHttpApplication()
    {

      // Create the Global / HttpApplication object
      Type appType = GetHttpApplicationType();
      _WebApplication = Activator.CreateInstance(appType) as HttpApplication;

      // Create the HttpApplicationState
      Type asType = typeof(HttpApplicationState);
      var appState = asType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null)
        .Invoke(new object[] { }) as HttpApplicationState;

      // Inject the application state
      var theField = typeof(HttpApplication).GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance);
      theField.SetValue(_WebApplication, appState);

      var mi = _WebApplication.GetType().GetMethod("Application_Start", BindingFlags.Instance | BindingFlags.NonPublic);
      mi.Invoke(_WebApplication, new object[] { _WebApplication, EventArgs.Empty });

  }

    /// <summary>
    /// Get the HttpApplication Type (typically named Global) from the application
    /// </summary>
    /// <returns></returns>
    internal Type GetHttpApplicationType()
    {

      // Force the web application to be loaded 
      _compiler.GetCompiledType("/Default.aspx");

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

    private static void CompleteHttpContext(HttpContext ctx)
    {

      // Socket State
      ctx.GetType().GetProperty("WebSocketTransitionState", BindingFlags.NonPublic | BindingFlags.Instance)
        .SetValue(ctx, (byte)2, null);

    }


    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          // TODO: dispose managed state (managed objects).
        }

        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        // TODO: set large fields to null.

        disposedValue = true;
      }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~WebApplicationManager() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      // TODO: uncomment the following line if the finalizer is overridden above.
      // GC.SuppressFinalize(this);
    }
    #endregion


  }

}
