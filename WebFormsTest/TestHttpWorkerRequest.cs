using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Fritz.WebFormsTest
{

  public class TestHttpWorkerRequest : HttpWorkerRequest
  {

    private string _rawRequestUrl;
    private Uri _RequestUri;
    private VirtualPathWrapper _absoluteUrl;

    public TestHttpWorkerRequest(string rawRequestUrl)
    {
      _rawRequestUrl = rawRequestUrl;
      _RequestUri = new Uri((rawRequestUrl.StartsWith("http") ? rawRequestUrl : "http://localhost") + rawRequestUrl);
      _absoluteUrl = VirtualPathWrapper.CreateAbsolute(VirtualPathUtility.ToAbsolute(rawRequestUrl));
    }

    public override void EndOfRequest()
    {
      throw new NotImplementedException();
    }

    public override void FlushResponse(bool finalFlush)
    {
      throw new NotImplementedException();
    }

    public override string GetAppPath()
    {
      return "/";
    }

    public override string GetAppPathTranslated()
    {
      return WebApplicationProxy.WebRootFolder;
    }

    public override string GetHttpVerbName()
    {
      throw new NotImplementedException();
    }

    public override string GetHttpVersion()
    {
      throw new NotImplementedException();
    }

    public override string GetLocalAddress()
    {
      return "127.0.0.1";
    }

    public override int GetLocalPort()
    {
      return 80;
    }

    public override string GetQueryString()
    {
      return _RequestUri.Query.Length <= 0 ? "" : _RequestUri.Query.Substring(1);
    }

    public override string GetRawUrl()
    {
      return _rawRequestUrl;
    }

    public override string GetRemoteAddress()
    {
      throw new NotImplementedException();
    }

    public override int GetRemotePort()
    {
      throw new NotImplementedException();
    }

    public override string GetUriPath()
    {

      // Drop everything after the ?
      string outPath = _absoluteUrl.VirtualPathString;

      if (outPath.Contains("?")) {
        outPath = outPath.Substring(0, outPath.IndexOf('?'));
      }

      return outPath;
    }

    public override string GetKnownRequestHeader(int index)
    {

      var _headers = new Dictionary<int, string>();
      _headers.Add(HeaderUserAgent, "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:43.0) Gecko/20100101 Firefox/43.0");

      if (_headers.ContainsKey(index))
        return _headers[index];

      return base.GetKnownRequestHeader(index);
    }

    public override void SendKnownResponseHeader(int index, string value)
    {
      throw new NotImplementedException();
    }

    public override void SendResponseFromFile(IntPtr handle, long offset, long length)
    {
      throw new NotImplementedException();
    }

    public override void SendResponseFromFile(string filename, long offset, long length)
    {
      throw new NotImplementedException();
    }

    public override void SendResponseFromMemory(byte[] data, int length)
    {
      throw new NotImplementedException();
    }

    public override void SendStatus(int statusCode, string statusDescription)
    {
      throw new NotImplementedException();
    }

    public override void SendUnknownResponseHeader(string name, string value)
    {
      throw new NotImplementedException();
    }
  }

}
