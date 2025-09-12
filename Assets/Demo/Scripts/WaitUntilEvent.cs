// /*
// Created by Darsan
// */

using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class WaitUntilEvent:WaitUntilEventBase
{

    public WaitUntilEvent(object target, string eventName) : base(target, eventName)
    {
    }

    // ReSharper disable once UnusedMember.Local
    void Callback()
    {
        CompleteAndRemoveEventHandler();
    }


}



public class WaitUntilEvent<T> : WaitUntilEventBase
{
    private readonly Action<T> _callback;

    public Tuple<T> Result { get; private set; }

    public WaitUntilEvent(object target, string eventName, Action<T> callback) : base(target, eventName)
    {
        _callback = callback;
    }

    public WaitUntilEvent(object target, string eventName) : base(target, eventName)
    {
    }

    // ReSharper disable once UnusedMember.Local
    void Callback(T result)
    {
        _callback?.Invoke(result);
        Result = new Tuple<T>(result);
        CompleteAndRemoveEventHandler();
    }
}


public abstract class WaitUntilEventBase:CustomYieldInstruction
{
    protected readonly object target;
    protected readonly EventInfo eventInfo;
    protected readonly Delegate eventDelegate;

    public bool IsCompleted { get; private set; }

    public override bool keepWaiting => !IsCompleted;

    protected WaitUntilEventBase(object target, string eventName)
    {
        this.target = target;
        eventInfo = target.GetType().GetEvent(eventName);
        eventDelegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, "Callback");
        eventInfo.AddEventHandler(target, eventDelegate);
    }

    public IEnumerator AsEnumerator()
    {
        yield return new WaitUntil(() => IsCompleted);
    }

    protected void CompleteAndRemoveEventHandler()
    {
        eventInfo.RemoveEventHandler(target, eventDelegate);
        IsCompleted = true;
    }

}

public class WaitUntilEvent<T, TJ> : WaitUntilEventBase
{
    private readonly Action<T, TJ> _callback;

    public Tuple<T, TJ> Result { get; private set; }

    public WaitUntilEvent(object target, string eventName, Action<T, TJ> callback) : base(target, eventName)
    {
        _callback = callback;
    }

    public WaitUntilEvent(object target, string eventName) : base(target, eventName)
    {
    }

    // ReSharper disable once UnusedMember.Local
    void Callback(T resultOne, TJ resultTwo)
    {
        _callback?.Invoke(resultOne, resultTwo);
        Result = new Tuple<T, TJ>(resultOne, resultTwo);
        CompleteAndRemoveEventHandler();
    }
}

// ReSharper disable once InconsistentNaming
public class WaitUntilEvent<T, TJ, J> : WaitUntilEventBase
{
    private readonly Action<T, TJ, J> _callback;

    public Tuple<T, TJ, J> Result { get; private set; }

    public WaitUntilEvent(object target, string eventName, Action<T, TJ, J> callback) : base(target, eventName)
    {
        _callback = callback;
    }

    public WaitUntilEvent(object target, string eventName) : base(target, eventName)
    {
    }

    // ReSharper disable once UnusedMember.Local
    void Callback(T resultOne, TJ resultTwo, J resultThree)
    {
        _callback?.Invoke(resultOne, resultTwo, resultThree);
        Result = new Tuple<T, TJ, J>(resultOne, resultTwo, resultThree);
        CompleteAndRemoveEventHandler();
    }
}