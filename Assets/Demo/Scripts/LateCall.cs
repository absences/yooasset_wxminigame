using System;
using System.Collections;
using UnityEngine;

public class LateCall : MonoBehaviour
{
    public delegate bool Condition();

    private bool _isMineGameObject;

    public void Call(Condition condition, Action onFinished = null)
    {
        StartCoroutine(CallAfterConditionCor(condition, () =>
        {
            CompleteAndDestroy(onFinished);
        }));
    }

    void CompleteAndDestroy(Action onFinished)
    {
        onFinished?.Invoke();
        if (_isMineGameObject)
            Destroy(gameObject);
        else
        {
            Destroy(this);
        }
    }

    IEnumerator CallAfterConditionCor(Condition condition, Action onFinished)
    {
        if (condition == null)
            yield break;
        while (!condition())
        {
            yield return null;
        }
        onFinished?.Invoke();
    }

    public void Cancel()
    {
        StopAllCoroutines();
        CompleteAndDestroy(null);
    }

    public static LateCall Create(GameObject go = null)
    {
        LateCall lateCall;
        if (!go)
        {
            go = new GameObject("LateCall");
            lateCall = go.AddComponent<LateCall>();
            lateCall._isMineGameObject = true;
        }
        else
        {
            lateCall = go.AddComponent<LateCall>();
        }

        return lateCall;
    }
}