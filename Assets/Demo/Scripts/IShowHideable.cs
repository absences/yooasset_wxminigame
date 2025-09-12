using System;

public interface IShowHideable
{
    event EventHandler<bool> ShowStateChanged;
    bool Showing { get; }
    ShowState CurrentShowState { get; }
    void Show(bool animate = true, Action onFinished = null);

    void Hide(bool animate = true, Action onFinished = null);
}