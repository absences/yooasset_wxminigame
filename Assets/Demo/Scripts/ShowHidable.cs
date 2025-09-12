using System;
using System.Collections.Generic;
using UnityEngine;

public class ShowHidable : MonoBehaviour, IShowHideable
{
    public event EventHandler<bool> ShowStateChanged;

    protected static readonly int SHOW_HASH = Animator.StringToHash("Show");
    protected static readonly int HIDE_HASH = Animator.StringToHash("Hide");

    [SerializeField] protected Animator anim;
    [SerializeField] protected List<GameObject> dependObjects = new List<GameObject>();
    protected ShowState currentShowState = ShowState.Hide;

    public bool Showing
    {
        get { return gameObject.activeSelf; }
        protected set
        {
            if (value == gameObject.activeSelf)
                return;

            gameObject.SetActive(value);
            ShowStateChanged?.Invoke(this, value);
        }
    }


    public ShowState CurrentShowState
    {
        get { return currentShowState; }
        protected set
        {
            if (currentShowState == value)
            {
                return;
            }

            if (!Showing && (value == ShowState.ShowAnimation || value == ShowState.Show))
                Showing = true;
            else if (Showing && (value == ShowState.Hide))
            {
                Showing = false;
            }

            currentShowState = value;
        }
    }

    // ReSharper disable once FlagArgument
    public virtual void Show(bool animate = true, Action completed = null)
    {
        if (Showing)
            throw new InvalidOperationException();

        CurrentShowState = ShowState.ShowAnimation;

        if (animate && anim != null)
        {
            anim.Play(SHOW_HASH);

            LateCall.Create(gameObject).Call(() =>
            {
                var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                return stateInfo.shortNameHash == SHOW_HASH && stateInfo.normalizedTime >= 0.99f;
            }, () =>
            {
                CurrentShowState = ShowState.Show;
                OnShowCompleted();
                completed?.Invoke();
            });
        }
        else
        {
            CurrentShowState = ShowState.Show;
            OnShowCompleted();
            completed?.Invoke();
        }

    }

    protected virtual void OnShowCompleted()
    {

    }

    protected virtual void OnEnable()
    {
        dependObjects.ForEach(o => o.SetActive(true));
    }

    protected virtual void OnDisable()
    {
        dependObjects.ForEach(o =>
        {
            if (o != null) o.SetActive(false);
        });
    }

    //     ReSharper disable once FlagArgument
    public virtual void Hide(bool animate = true, Action completed = null)
    {
        if (!Showing)
            throw new InvalidOperationException();
        CurrentShowState = ShowState.HideAnimation;
        if (animate && anim != null)
        {
            anim.Play(HIDE_HASH);

            LateCall.Create(gameObject).Call(() =>
            {
                var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                return stateInfo.shortNameHash == HIDE_HASH && stateInfo.normalizedTime >= .99f;
            }, () =>
            {

                CurrentShowState = ShowState.Hide;
                OnHideCompleted();
                completed?.Invoke();
            });
        }
        else
        {
            CurrentShowState = ShowState.Hide;
            completed?.Invoke();
        }
    }

    protected virtual void OnHideCompleted()
    {

    }

    public void Exit()
    {
        Hide();
    }
}