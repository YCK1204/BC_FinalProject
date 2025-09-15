using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UiState
{
    Hidden,
    Updating,
    NormalView,
    WideView,
    FullScreen
}

public abstract class UiStateBase
{
    protected UIController ui;

    public UiStateBase(UIController controller)
    {
        this.ui = controller;
    }

    public abstract void Enter();
    public abstract void Exit();
}