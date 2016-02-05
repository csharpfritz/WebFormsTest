using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace Fritz.WebFormsTest.Internal
{

  public class TestHttpSessionStateContainer : IHttpSessionState
  {
    private bool _Abandon = false;
    private readonly Dictionary<string, object> _Items = new Dictionary<string, object>();

    public TestHttpSessionStateContainer(string sessionId)
    {
      this.SessionID = sessionId;
      this.Timeout = 300; /// 10 minutes
    }

    public object this[int index]
    {
      get
      {
        return _Items.Skip(index).Take(1);
      }

      set
      {
        // TODO: Implement
        throw new NotImplementedException();
      }
    }

    public object this[string name]
    {
      get
      {
        return _Items[name];
      }

      set
      {
        _Items.Remove(name);
        _Items.Add(name, value);
      }
    }

    public int CodePage
    {
      get
      {
        return HttpContext.Current.Response.ContentEncoding.CodePage;
      }

      set
      {
        if (HttpContext.Current != null)
          HttpContext.Current.Response.ContentEncoding = Encoding.GetEncoding(value);
      }
    }

    public HttpCookieMode CookieMode
    {
      get
      {
        // We're in test mode -- push sessionId on URI if its used at all
        return HttpCookieMode.UseUri;
      }
    }

    public int Count
    {
      get
      {
        return _Items.Count;
      }
    }

    public bool IsCookieless
    {
      get
      {
        return CookieMode == HttpCookieMode.UseUri;
      }
    }

    public bool IsNewSession
    {
      get
      {
        // NOTE: Not sure this is needed
        return false;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        // Always allow edits in test mode
        return false;
      }
    }

    public bool IsSynchronized
    {
      get
      {
        // Same implementation as System.Web.HttpSessionStateContainer
        return false;
      }
    }

    public NameObjectCollectionBase.KeysCollection Keys
    {
      get
      {
        // TODO: Implement a keys collection
        throw new NotImplementedException();
      }
    }

    public int LCID
    {
      get { return Thread.CurrentThread.CurrentCulture.LCID; }
      set { Thread.CurrentThread.CurrentCulture = CultureInfo.ReadOnly(new CultureInfo(value)); }
    }

    public SessionStateMode Mode
    {
      get
      {
        return SessionStateMode.InProc;
      }
    }

    public string SessionID
    {
      get; private set;
    }

    public HttpStaticObjectsCollection StaticObjects
    {
      get
      {
        // TODO: Implement StaticObjectsCollection - not sure how effective this will be
        throw new NotImplementedException();
      }
    }

    public object SyncRoot
    {
      get
      {
        return this;
      }
    }

    public int Timeout
    {
      get; set;
    }

    public void Abandon()
    {
      _Abandon = true;
    }

    public void Add(string name, object value)
    {
      _Items.Add(name, value);
    }

    public void Clear()
    {
      _Items.Clear();
    }

    public void CopyTo(Array array, int index)
    {
      _Items.ToArray().CopyTo(array, index);
    }

    public IEnumerator GetEnumerator()
    {
      return _Items.GetEnumerator();
    }

    public void Remove(string name)
    {
      _Items.Remove(name);
    }

    public void RemoveAll()
    {
      _Items.Clear();
    }

    public void RemoveAt(int index)
    {
      // TODO: Handle position interactions
      throw new NotImplementedException();
    }
  }

}
