using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Hosting;

namespace Fritz.WebFormsTest.Internal
{

  /// <summary>
  /// A class to proxy configuration into the non-public capabilities of the HostingEnvironment
  /// </summary>
  internal class HostingEnvironmentWrapper
  {

    private static readonly HostingEnvironmentWrapper _Instance;
    private DummyRegisteredObject _DummyRegisteredObject;
    private readonly HostingEnvironment _Inner;
    private readonly Type _Type = typeof(HostingEnvironment);
    private readonly TestVirtualPathProvider _VirtualPathProvider = new TestVirtualPathProvider();


    public HostingEnvironmentWrapper()
    {

      _Inner = new HostingEnvironment();

      Configure();

      RegisterDummyObject();

    }

    /// <summary>
    /// Register a dummy object that will prevent the hosting environment from being torn down
    /// by the ASP.NET management process during testing
    /// </summary>
    private void RegisterDummyObject()
    {
      _DummyRegisteredObject = new DummyRegisteredObject();
      HostingEnvironment.RegisterObject(_DummyRegisteredObject);
    }

    private void Configure()
    {

      var fi = _Type.GetField("_appVirtualPath", BindingFlags.Instance | BindingFlags.NonPublic);
      fi.SetValue(_Inner, VirtualPathWrapper.Create("/").VirtualPath);

      fi = _Type.GetField("_configMapPath", BindingFlags.Instance | BindingFlags.NonPublic);
      fi.SetValue(_Inner, new TestConfigMapPath());

      fi = _Type.GetField("_virtualPathProvider", BindingFlags.Instance | BindingFlags.NonPublic);
      fi.SetValue(_Inner, _VirtualPathProvider);

    }

    public static implicit operator HostingEnvironment(HostingEnvironmentWrapper wrapper)
    {

      return HostingEnvironmentWrapper._Instance._Inner;

    }

    internal class DummyRegisteredObject : IRegisteredObject
    {
      public void Stop(bool immediate)
      {
        return;
      }
    }

  }

}
