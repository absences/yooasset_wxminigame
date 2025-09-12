// /*
// Created by Darsan
// */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class Tutorial
{
    public event Action<Tutorial> Completed;

    public abstract void Execute();

    public IEnumerator AsExecuteEnumerator()
    {
        var completed = false;
        Completed += tutorial => completed = true;
        Execute();
        yield return new WaitUntil(() => completed);
    }


    public void Complete()
    {
        Completed?.Invoke(this);
    }

}

public class SimpleTutorial : Tutorial
{
    private readonly Action<Action> _action;

    public SimpleTutorial(Action<Action> action)
    {
        _action = action;
    }

    public override void Execute()
    {
        if (_action == null)
        {
            throw new InvalidOperationException();
        }

        _action(Complete);
    }

    public static SimpleTutorial Create(IEnumerable<Tutorial> tutorials, float timeDifference = 0)
    {
        return new SimpleTutorial(action =>
        {
            SimpleCoroutine.Create().Coroutine(SimpleCoroutine.MergeSequence(tutorials
                .Select(tutorial => tutorial.AsExecuteEnumerator()), action, timeDifference));
        });
    }
}