using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class InteractEventListener : MonoBehaviour
{
    public event UnityAction OnHover;
    public event UnityAction OnEnter;
    public event UnityAction OnExit;
    public event UnityAction OnClick;

    public void Hover()
    {
        if (OnHover != null) OnHover();
    }

    public void Enter()
    {
        if (OnEnter != null) OnEnter();
    }

    public void Exit()
    {
        if (OnExit != null) OnExit();
    }

    public void Click()
    {
        if (OnClick != null) OnClick();
    }
}