using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Configuration;

namespace Fritz.WebFormsTest.Internal
{

  /// <summary>
  /// An internal class for the configuration of the ConfigMapPath
  /// </summary>
  internal class TestConfigMapPath : IConfigMapPath
  {
    public string GetAppPathForPath(string siteID, string path)
    {
      throw new NotImplementedException();
    }

    public void GetDefaultSiteNameAndID(out string siteName, out string siteID)
    {
      throw new NotImplementedException();
    }

    public string GetMachineConfigFilename()
    {
      throw new NotImplementedException();
    }

    public void GetPathConfigFilename(string siteID, string path, out string directory, out string baseName)
    {
      throw new NotImplementedException();
    }

    public string GetRootWebConfigFilename()
    {
      throw new NotImplementedException();
    }

    public string MapPath(string siteID, string path)
    {

      // Simple map path using the folder from WebApplicationProxy as root
      var root = WebApplicationProxy.WebRootFolder;

      path = path.Replace('/','\\');
      path = path.StartsWith("\\") ? path.Substring(1) : path;

      return Path.Combine(root, path);

    }

    public void ResolveSiteArgument(string siteArgument, out string siteName, out string siteID)
    {
      throw new NotImplementedException();
    }
  }

}
