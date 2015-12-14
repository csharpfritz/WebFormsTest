using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Web.UI;

namespace WebFormsTest
{

  /// <summary>
  /// Handle the AutoEventWireup feature of Pages and Controls
  /// </summary>
  internal class AutoEventHandler
  {

    private AutoEventHandler(TestablePage systemUnderTest)
    {

      UnderTest = systemUnderTest;

    }

    public TestablePage UnderTest { get; private set; }

    /// <summary>
    /// Entrance point to connect events to methods
    /// </summary>
    public void Connect(TestablePage sut)
    {

      var handler = new AutoEventHandler(sut);

      handler.GetDelegateInformation();

    }

    internal delegate void VoidMethod();

    private void GetDelegateInformation()
    {

      var eventsToInspect = new[] { AutoEvents.InitEventName, AutoEvents.LoadEventName, AutoEvents.LoadCompleteEventName, AutoEvents.PreRenderCompleteEventName, AutoEvents.InitCompleteEventName, AutoEvents.SaveStateCompleteEventName };

      Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();

      foreach (var e in eventsToInspect)
      {

        MethodInfo mi = GetInstanceMethodInfo(typeof(EventHandler), e);
        if (mi != null)
        {
          methods.Add(e, mi);
          continue;
        } else
        {
          mi = GetInstanceMethodInfo(typeof(VoidMethod), e);
          if (mi != null) methods.Add(e, mi);
        }

      }

      // Exit now if nothing to do
      if (methods.Count == 0) return;
      var eventExists = false;

      foreach (var eventHandler in methods)
      {

        Delegate eventDelegates = UnderTest.Events[eventHandler.Key];
        if (eventDelegates != null)
        {
          foreach (var eventDelegate in eventDelegates.GetInvocationList())
          {
            if (eventDelegate.Method.Equals(eventHandler.Value))
            {
              eventExists = true;
              break;
            }
          }
        }

        // If the event exists, generate the event addHandler code
        if (eventExists)
        {
          // Createa a new delegate proxy
          var functionPtr = eventHandler.Value.MethodHandle.GetFunctionPointer();
          var newHandler = new DelegateProxy(UnderTest, functionPtr).Handler;
          UnderTest.Events.AddHandler(eventHandler.Key, newHandler);

          /// NEED TO WALK REFLECTION TO GET THE EVENTHANDLER AND EMIT THE event += EventHandler code
          /// http://referencesource.microsoft.com/#System.Web/Util/ArglessEventHandlerProxy.cs,ff9a890000de7671
          /// http://referencesource.microsoft.com/#System.Web/UI/TemplateControl.cs,be9a232ff171c33b
          //var handler = (new CalliEventHandlerDelegateProxy())
        }

      }

    }

    private MethodInfo GetInstanceMethodInfo(Type delegateType, string methodName)
    {
      Delegate del = Delegate.CreateDelegate(
          type: delegateType,
          target: UnderTest,
          method: methodName,
          ignoreCase: true,
          throwOnBindFailure: false);

      return (del != null) ? del.Method : null;
    }

    public static class AutoEvents
    {
      public const string PreInitEventName = "Page_PreInit";
      public const string InitEventName = "Page_Init";
      public const string InitCompleteEventName = "Page_InitComplete";
      public const string LoadEventName = "Page_Load";
      public const string PreLoadEventName = "Page_PreLoad";
      public const string LoadCompleteEventName = "Page_LoadComplete";
      public const string PreRenderCompleteEventName = "Page_PreRenderComplete";
      public const string PreRenderCompleteAsyncEventName = "Page_PreRenderCompleteAsync";
      public const string DataBindEventName = "Page_DataBind";
      public const string PreRenderEventName = "Page_PreRender";
      public const string SaveStateCompleteEventName = "Page_SaveStateComplete";
      public const string UnloadEventName = "Page_Unload";
      public const string ErrorEventName = "Page_Error";
      public const string AbortTransactionEventName = "Page_AbortTransaction";
      public const string OnTransactionAbortEventName = "OnTransactionAbort";
      public const string CommitTransactionEventName = "Page_CommitTransaction";
      public const string OnTransactionCommitEventName = "OnTransactionCommit";

    }

    internal delegate void ParameterfulDelegate(object sender, EventArgs args);

    private class DelegateProxy
    {

      private TestablePage _Target;
      private IntPtr _FunctionPtr;

      public DelegateProxy(TestablePage sut, IntPtr functionPtr)
      {
        _Target = sut;
        _FunctionPtr = functionPtr;
      }

      internal void Callback(object sender, EventArgs e)
      {
        var del = CreateDelegate(_Target, _FunctionPtr);
        del(sender, e);
      }

      private ParameterfulDelegate CreateDelegate(TestablePage _Target, IntPtr _FunctionPtr)
      {

        var ctor = typeof(ParameterfulDelegate).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) });
        var method = new DynamicMethod(
            name: "TestDelegate_" + 
        );



      }

      public EventHandler Handler
      {
        get
        {
          return new EventHandler(Callback);
        }
      }
    }
  }

}
