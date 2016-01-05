using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class InteractEventHandler : MonoBehaviour
{
    protected InteractEventListener _EventListener;
    protected bool _isInit;

    protected virtual void Init()
    {
        if (!_isInit)
        {
            _EventListener = GetComponent<InteractEventListener>();
            _EventListener.OnEnter += InputEnter;
            _EventListener.OnHover += InputHover;
            _EventListener.OnExit += InputExit;
            _EventListener.OnClick += InputClick;
        }
    }

    protected virtual void InputEnter()
    {
    }

    protected virtual void InputHover()
    {
    }

    protected virtual void InputExit()
    {
    }

    protected virtual void InputClick()
    {
    }

    public virtual void Interact()
    {
        
    }

    public virtual void Interact(UnityAction onComplete)
    {
        if (onComplete != null) onComplete();
    }
}