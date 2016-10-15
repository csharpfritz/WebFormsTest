using Fritz.WebFormsTest.Internal;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;

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
    private bool _SkipCrawl = false;
    private bool Initialized = false;
    private bool _SkipPrecompile = true;
    private static HostingEnvironmentWrapper _hostingEnvironment;

    // Identical to SessionStateUtility.SESSION_KEY
    private const String SESSION_KEY = "AspSession";
    private static DummyRegisteredObject _DummyRegisteredObject;

    /// <summary>
    /// Create a proxy for the web application to be inspected
    /// </summary>
    /// <param name="rootFolder">Physical location on disk of the web application source code</param>
    private WebApplicationProxy(string rootFolder, bool skipCrawl, bool skipPrecompile)
    {

      WebRootFolder = rootFolder;
      _SkipCrawl = skipCrawl;
      _SkipPrecompile = skipPrecompile;

    }

    ~WebApplicationProxy()
    {
      Dispose(false);
    }


    /// <summary>
    /// Create a Proxy to the Web Application that contains the type submitted
    /// </summary>
    /// <param name="sampleWebType">A sample type from the assembly of the web project to be tested</param>
    /// <param name="options">Options to configure the proxy</param>
    public static void Create(Type sampleWebType, WebApplicationProxyOptions options = null)
    {
      DirectoryInfo webFolder;

      if (options != null && options.WebFolder != null)
      {
        webFolder = new DirectoryInfo(options.WebFolder);
      }
      else
      {
        webFolder = LocateSourceFolder(sampleWebType);
      }

      Create(webFolder.FullName, options?.SkipCrawl ?? true, options?.SkipPrecompile ?? true);
    }

    private static void Create(string rootFolder, bool skipCrawl = true, bool skipPrecompile = true)
    {
      _Instance = new WebApplicationProxy(rootFolder, skipCrawl, skipPrecompile);

      ReadWebConfig();

      InjectTestValuesIntoHttpRuntime();

      _hostingEnvironment = new HostingEnvironmentWrapper();

      _DummyRegisteredObject = new DummyRegisteredObject();
      HostingEnvironment.RegisterObject(_DummyRegisteredObject);

      SubstituteDummyHttpContext("/");

      _Instance.InitializeInternal();

    }

    /// <summary>
    /// Inspect the application and create mappings for all types
    /// </summary>
    private void InitializeInternal()
    {

      if (Initialized) return;

      // Can this go async?
      _compiler = new ClientBuildManager(@"/",
        WebRootFolder,
        null,
        new ClientBuildManagerParameter()
        {
        });

      SetHttpRuntimeAppDomainAppPath();

      // In larger suites, this may be more cost effective to run
      if (!_SkipPrecompile)
      {
        _compiler.PrecompileApplication();
      }

      ConfigureBuildManager();

      // Async?
      CrawlWebApplication();

      CreateHttpApplication();

      Initialized = true;

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

    private void SetHttpRuntimeAppDomainAppPath()
    {
      var p = typeof(HttpRuntime).GetField("_appDomainAppPath", BindingFlags.NonPublic | BindingFlags.Instance);
      p.SetValue(HttpRuntimeInstance, WebApplicationProxy.WebPrecompiledFolder);

      //var getAppDomainMethod = typeof(HttpRuntime).GetMethod("GetAppDomainString", BindingFlags.NonPublic | BindingFlags.Static);
      //var appId = getAppDomainMethod.Invoke(null, new object[] { ".appId" });
      var appId = _compiler.GetType().GetField("_appId", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_compiler);

      p = typeof(HttpRuntime).GetField("_appDomainAppId", BindingFlags.NonPublic | BindingFlags.Instance);
      p.SetValue(HttpRuntimeInstance, appId);

      p = typeof(HttpRuntime).GetField("_codegenDir", BindingFlags.NonPublic | BindingFlags.Instance);
      p.SetValue(HttpRuntimeInstance, _compiler.CodeGenDir);

      p = typeof(HttpRuntime).GetField("_DefaultPhysicalPathOnMapPathFailure", BindingFlags.NonPublic | BindingFlags.Static);
      p.SetValue(null, WebApplicationProxy.WebRootFolder);

    }

    /// <summary>
    /// Check if the Proxy is running
    /// </summary>
    public static bool IsInitialized { get { return _Instance != null ? _Instance.Initialized : false; } }

    /// <summary>
    /// Add some needed configuration to the HttpRuntime so that it thinks we are running in a Web Service
    /// </summary>
    private static void InjectTestValuesIntoHttpRuntime()
    {

      var p = typeof(HttpRuntime).GetField("_appDomainAppVPath", BindingFlags.NonPublic | BindingFlags.Instance);
      p.SetValue(HttpRuntimeInstance, VirtualPathWrapper.Create("/").VirtualPath);

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
    public static object GetPageByLocation(string location, Action<HttpContext> contextModifiers = null)
    {
      return GetPageByLocation(location, BrowserDefinitions.DEFAULT, contextModifiers);
    }

    /// <summary>
    /// Get a fully instantiated Page at the web location specified
    /// </summary>
    /// <param name="location">The web absolute folder location to retrive the Page from</param>
    /// <returns>The Page object from the specified location</returns>
    public static object GetPageByLocation(string location, HttpBrowserCapabilities browserCaps, Action<HttpContext> contextModifiers = null)
    {

      if (_Instance == null || !_Instance.Initialized) throw new InvalidOperationException("The WebApplicationProxy has not been created and initialized properly");

      location = HandleFriendlyUrls(location);

      var returnType = _Instance._compiler.GetCompiledType(location);
      SubstituteDummyHttpContext(location);

      if (contextModifiers != null) contextModifiers(HttpContext.Current);

      dynamic outObj = Activator.CreateInstance(returnType);

      // Prepare the page for testing
      if (outObj is Page) ((Page)outObj).PrepareForTest();

      CompleteHttpContext(HttpContext.Current);

      return outObj;


    }

    /// <summary>
    /// Get a fully instantiated Page at the web location specified
    /// </summary>
    /// <typeparam name="T">The type of the Page to fetch</typeparam>
    /// <param name="location">The web absolute folder location to retrive the Page from</param>
    /// <returns>A strongly-typed Page object from the specified location</returns>
    public static T GetPageByLocation<T>(string location, Action<HttpContext> contextModifiers = null) where T : Page
    {

      return GetPageByLocation(location, contextModifiers) as T;

    }

    /// <summary>
    /// Get a fully instantiated Page at the web location specified
    /// </summary>
    /// <typeparam name="T">The type of the Page to fetch</typeparam>
    /// <param name="location">The web absolute folder location to retrive the Page from</param>
    /// <returns>A strongly-typed Page object from the specified location</returns>
    public static T GetPageByLocation<T>(string location, HttpBrowserCapabilities browserCaps, Action<HttpContext> contextModifiers = null) where T : Page
    {

      return GetPageByLocation(location, browserCaps, contextModifiers) as T;

    }

    public static T GetPageByType<T>() where T : Page
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

    public static T GetService<T>(Action<HttpContext> contextModifier = null, params object[] serviceConstructorArgs ) where T : WebService
    {

      var svc = Activator.CreateInstance(typeof(T), serviceConstructorArgs) as T;
      InjectContextIntoWebService(svc, contextModifier);

      return svc;

    }


    /// <summary>
    /// Allows for a HttpContext to be injected into a ASMX service when testing a service that is using a context, a session, and other web related properties
    /// that one would expect to be present while under test.
    /// </summary>
    /// <typeparam name="TService">A service that is typeof WebService from Microsoft's System.Web.Services namespace.</typeparam>
    /// <param name="service">The service to inject context into.</param>
    /// <param name="contextModifier">An optional action to modify the context if need be such as adding a session for example.</param>
    private static void InjectContextIntoWebService<TService>(TService service, Action<HttpContext> contextModifier = null) where TService : WebService
    {
      var root = WebRootFolder.Split('\\');
      var fullPath = $"/{service.GetType().FullName}".Replace(".", "/").Split('/');

      // Attempting to compute the relative path of the service that we want to inject the context into.
      // This probably does not matter as much compared to aspx pages since we don't have to do any compiling at runtime
      // but am making an effort to be consistent.
      var relativeFilePath = string.Join("/", fullPath.Except(root)) + ".asmx";

      // Construct our dummy http context.
      SubstituteDummyHttpContext(relativeFilePath);

      // Invoke our context modifier if the action was given to us.
      contextModifier?.Invoke(HttpContext.Current);

      // Finally, get the non-public SetContext method definition from the web service type so that we can invoke it on our service.
      var setContextMethod = typeof(WebService).GetMethod("SetContext", BindingFlags.Instance | BindingFlags.NonPublic);
      setContextMethod.Invoke(service, new object[] { HttpContext.Current });
    }

    /// <summary>
    /// This is the application object created typically by the "global.asax.cs" class
    /// </summary>
    public static HttpApplication Application
    {
      get; private set;
    }

    public static Cache Cache
    {
      get
      {

        // if (HttpContext.Current == null) SubstituteDummyHttpContext();
        return HttpContext.Current.Cache;

      }
    }

    /// <summary>
    /// The physical location of the web application on disk.  c:\dev\MyWebApp
    /// </summary>
    public static string WebRootFolder
    {
      get; private set;
    }

    /// <summary>
    /// Gets a new session and associates it with HttpContext.Current
    /// </summary>
    /// <returns></returns>
    public static HttpSessionState GetNewSession(Dictionary<string, object> initialItems = null)
    {

      var sessionId = Guid.NewGuid().ToString();
      var sessionContainer = new TestHttpSessionStateContainer(sessionId);

      var ctor = typeof(HttpSessionState).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(IHttpSessionState) }, null);
      var newSession = ctor.Invoke(new object[] { sessionContainer }) as HttpSessionState;

      if (initialItems != null)
      {
        foreach (var item in initialItems)
        {
          newSession.Add(item.Key, item.Value);
        }
      }

      return newSession;


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
      try
      {
        // Directory.Delete(WebApplicationRootFolder, true);
      }
      catch (DirectoryNotFoundException)
      {
        // Its ok...  its already gone!
      }
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

    private static void CompleteHttpContext(HttpContext ctx)
    {

      // Socket State
      ctx.GetType().GetProperty("WebSocketTransitionState", BindingFlags.NonPublic | BindingFlags.Instance)
        .SetValue(ctx, (byte)2, null);

    }

    public static string WebPrecompiledFolder
    {
      get { return _Instance._compiler.CodeGenDir; }
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
      var blacklist = new string[] { "Microsoft.", "System", "mscorlib", "xunit." };

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

    private static DirectoryInfo LocateSourceFolder(Type sampleWebType)
    {
		DirectoryInfo webFolder;
		var parms = new ReaderParameters { ReadSymbols = true };
		var assemblyDef = AssemblyDefinition.ReadAssembly(new Uri(sampleWebType.Assembly.CodeBase).LocalPath);

		byte[] debugHeader;
		var img = assemblyDef.MainModule.GetDebugHeader(out debugHeader);
		
		// Skipping the first n number of bytes from the debugHeader could lead to unexpected results.
		// It seems that all we are really interested in is finding the pdb file in the header, a regex will work 
		// great for this.
		var debugHeaderValue = Encoding.ASCII.GetString(debugHeader);

		// The regex pattern to pull pdb file
		// Look for either a file stored at any Windows letter path or unc windows path followed by n words and ending with '.pdb'.
		var regexPattern = "(?<pdbFile>([a-zA-Z]:\\\\|\\\\\\\\)[\\w._\\\\0-9-\\s]*.pdb)";

		// See if pattern matches
		var match = Regex.Match(debugHeaderValue, regexPattern);

		if (!match.Success)
			throw new Exception($"Could not find pdb file from assembly in type: {sampleWebType.FullName}");

		var pdbFilePath = match.Groups["pdbFile"].Value;
		var pdbFile = new FileInfo(pdbFilePath);
		
		webFolder = pdbFile.Directory;

		while ((webFolder.GetFiles("*.csproj").Length == 0 && webFolder.GetFiles("*.vbproj").Length == 0))
		{
		webFolder = webFolder.Parent;
		}

		return webFolder;
    }

    private static void TriggerApplicationStart(HttpApplication app)
    {

      var mi = app.GetType().GetMethod("Application_Start", BindingFlags.Instance | BindingFlags.NonPublic);
      mi.Invoke(app, new object[] { app, EventArgs.Empty });
    }

    private static void CreateHttpApplication()
    {
      // Create the Global / HttpApplication object
      Type appType = GetHttpApplicationType();
      try
      {
        Application = Activator.CreateInstance(appType) as HttpApplication;
      }
      catch (Exception ex)
      {
        throw new WebApplicationProxyException("Failed to set Application property on WebApplicationProxy; could not create instance from your application that is implementing the HttpApplication type. Please check the class in your appliction that is implementing the HttpApplication type; the default constructor is throwing an exception.", ex);
      }

      // Create the HttpApplicationState
      Type asType = typeof(HttpApplicationState);
      var appState = asType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null)
                 .Invoke(new object[] { }) as HttpApplicationState;

      // Inject the application state
      var theField = typeof(HttpApplication).GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance);
      theField.SetValue(Application, appState);

      TriggerApplicationStart(Application);
    }

    private static void ReadWebConfig()
    {

      AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", Path.Combine(WebRootFolder, "web.config"));
      //      WebConfigurationManager.OpenWebConfiguration(Path.Combine(WebRootFolder, "web.con‌​fig"));

    }

    private static string HandleFriendlyUrls(string location)
    {

      var legitExtensions = new[] { ".aspx", ".master", ".ascx", ".ashx", ".asmx" };

      // Return immediately if this already has an ASPX in it
      if (legitExtensions.Any(e => location.ToLowerInvariant().Contains(e)) )
        return location;

      var folders = location.Split('/');
      var currentLocation = "";
      var mpProvider = new TestConfigMapPath();
      foreach (var folder in folders)
      {

        if (string.IsNullOrEmpty(folder)) continue;

        currentLocation += "/" + folder;
        var absoluteLocation = mpProvider.MapPath("", currentLocation);
        if (Directory.Exists(absoluteLocation)) continue;
        if (File.Exists(absoluteLocation + ".aspx")) {
          break;
        }

        throw new FileNotFoundException("Unable to locate the file requested", location);

      }

      return currentLocation + ".aspx";

    }

    #endregion

    internal class DummyRegisteredObject : IRegisteredObject
    {
      public void Stop(bool immediate)
      {
        return;
      }
    }

  }



}
