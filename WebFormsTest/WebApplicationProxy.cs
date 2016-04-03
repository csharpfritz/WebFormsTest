using Fritz.WebFormsTest.Internal;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.SessionState;
using System.Web.UI;

namespace Fritz.WebFormsTest
{

  /// <summary>
  /// A class that wraps a web application project and provides access to the .NET code contained within
  /// </summary>
  public class WebApplicationProxy : MarshalByRefObject, IDisposable
  {

    private static readonly Dictionary<Type, string> _LocationTypeMap = new Dictionary<Type, string>();

    // Identical to SessionStateUtility.SESSION_KEY
    private const String SESSION_KEY = "AspSession";
    private static AppDomain _ShadowDomain;
    internal static WebApplicationManager _WebAppManager;

    /// <summary>
    /// Create a proxy for the web application to be inspected
    /// </summary>
    /// <param name="rootFolder">Physical location on disk of the web application source code</param>
    private WebApplicationProxy()
    {

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

      if (options != null && options.PhysicalRootFolder != null)
      {
        webFolder = new DirectoryInfo(options.PhysicalRootFolder);
      }
      else {

        webFolder = LocateSourceFolder(sampleWebType);

        Create(webFolder.FullName, options?.SkipCrawl ?? true, options?.SkipPrecompile ?? true);

      }

    }

    [Obsolete("Use the autolocate-enabled signature")]
    public static void Create(string rootFolder, bool skipCrawl = true, bool skipPrecompile = true)
    {

      // Hide the instance of the WebApplication proxy in another domain so that it behaves more like a web app
      _ShadowDomain = AppDomain.CreateDomain("TestDomain", null, AppDomain.CurrentDomain.BaseDirectory, null, false);
      Debug.WriteLine($"base directory: {_ShadowDomain.BaseDirectory}");

      WebRootFolder = rootFolder;

      var thisType = typeof(WebApplicationManager);
      var thisRef = _ShadowDomain.CreateInstance(thisType.Assembly.FullName, thisType.FullName);
      _WebAppManager = thisRef.Unwrap() as WebApplicationManager;
      _WebAppManager.Initialize(new WebApplicationProxyOptions
      {
        PhysicalRootFolder = rootFolder,
        SkipCrawl = skipCrawl,
        SkipPrecompile = skipPrecompile
      });

      Debug.WriteLine($"WebAppMgr IsInitialized: {_WebAppManager.IsInitialized()}");
            
    }

    /// <summary>
    /// Check if the Proxy is running
    /// </summary>
    public static bool IsInitialized {  get { return _WebAppManager != null ? _WebAppManager.IsInitialized() : false; } }

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

      Debug.WriteLine($"Domain: {AppDomain.CurrentDomain.FriendlyName}");

      if (_WebAppManager == null || !_WebAppManager.IsInitialized()) throw new InvalidOperationException("The WebApplicationProxy has not been created and initialized properly");

      // Redirecting to the instance inner method to allow this to take place in the Shadow AppDomain
      return _WebAppManager.GetByLocation(location, browserCaps, contextModifiers);

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

      throw new InvalidOperationException("Unable to fetch by type because the project has not been indexed");

      //var locn = _Instance._LocationTypeMap[typeof(T)];

      //return GetPageByLocation<T>(locn);

    }

    public static object GetPageByType(Type pageType)
    {

      throw new InvalidOperationException("Unable to fetch by type because the project has not been indexed");

      //var locn = _Instance._LocationTypeMap[pageType];

      //return GetPageByLocation(locn);

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
      _WebAppManager.Dispose();
    }

    public void Dispose()
    {
      Dispose(true);
    }

    protected virtual void Dispose(bool isDisposing)
    {

      if (isDisposing) GC.SuppressFinalize(this);

      _WebAppManager.Dispose();
      AppDomain.Unload(_ShadowDomain);

    }

    #region Private Methods

    private static DirectoryInfo LocateSourceFolder(Type sampleWebType)
    {
      DirectoryInfo webFolder;
      var parms = new ReaderParameters { ReadSymbols = true };
      var assemblyDef = AssemblyDefinition.ReadAssembly(new Uri(sampleWebType.Assembly.CodeBase).LocalPath);

      byte[] debugHeader;
      var img = assemblyDef.MainModule.GetDebugHeader(out debugHeader);
      var pdbFilename = Encoding.ASCII.GetString(debugHeader.Skip(24).Take(debugHeader.Length - 25).ToArray());
      var pdbFile = new FileInfo(pdbFilename);
      webFolder = pdbFile.Directory;

      while ((webFolder.GetFiles("*.csproj").Length == 0 && webFolder.GetFiles("*.vbproj").Length == 0))
      {
        webFolder = webFolder.Parent;
      }

      return webFolder;
    }


    private static void ReadWebConfig()
    {

      // NOTE: This needs to happen in the shadow AppDomain
      AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", Path.Combine(WebRootFolder, "web.config"));

    }

    #endregion

  }



}
