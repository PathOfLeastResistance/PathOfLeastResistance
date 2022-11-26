using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityTools;

public class MouseDragEventArgs : EventArgs
{
    public Vector2 MousePosition { get; private set; }
    public Vector2 MouseDelta { get; private set; }

    public MouseDragEventArgs(Vector2 mousePosition, Vector2 mouseDelta)
    {
        MousePosition = mousePosition;
        MouseDelta = mouseDelta;
    }
}

public class SimpleMouseInput : SingletonMonobehaviour<SimpleMouseInput>
{
    private GameInputActions m_Actions;
    private bool m_IsDown = false;
    public event Action<float> OnWheelEvent;

    void OnEnable()
    {
        m_Actions = new GameInputActions();
        m_Actions.GameActions.Enable();

        m_Actions.GameActions.ScrollAction.performed += OnMouseWheel;

        m_Actions.GameActions.LeftButton.started += LeftDown;
        m_Actions.GameActions.LeftButton.canceled += LeftUp;

        m_Actions.GameActions.RightButton.started += RightButtonDown;
        m_Actions.GameActions.RightButton.canceled += RightButtonUp;

        m_Actions.GameActions.MouseMove.performed += OnMouseMove;
    }

    private int m_ActiveButton = -1;
    private bool m_DragStarted = false;
    private Vector2 m_mousePos = Vector2.zero;
    private Vector2 m_prevMousePos = Vector2.zero;

    //Left button drags
    private event Action<MouseDragEventArgs> OnLeftDragStartEvent;
    private event Action<MouseDragEventArgs> OnLeftDragEvent;
    private event Action<MouseDragEventArgs> OnLeftDragEndEvent;

    //Right button drags
    private event Action<MouseDragEventArgs> OnRightDragStartEvent;
    private event Action<MouseDragEventArgs> OnRightDragEvent;
    private event Action<MouseDragEventArgs> OnRightDragEndEvent;

    public IDisposable SubscribeLeftButtonDrag(Action<MouseDragEventArgs> dragStart, Action<MouseDragEventArgs> dragPerformed, Action<MouseDragEventArgs> dragEnd)
    {
        OnLeftDragStartEvent += dragStart;
        OnLeftDragEvent += dragPerformed;
        OnLeftDragEndEvent += dragEnd;

        return new DisposableAction(() =>
        {
            OnLeftDragStartEvent -= dragStart;
            OnLeftDragEvent -= dragPerformed;
            OnLeftDragEndEvent -= dragEnd;
        });
    }
    
    public IDisposable SubscribeRightButtonDrag(Action<MouseDragEventArgs> dragStart, Action<MouseDragEventArgs> dragPerformed, Action<MouseDragEventArgs> dragEnd)
    {
        OnRightDragStartEvent += dragStart;
        OnRightDragEvent += dragPerformed;
        OnRightDragEndEvent += dragEnd;

        return new DisposableAction(() =>
        {
            OnRightDragStartEvent -= dragStart;
            OnRightDragEvent -= dragPerformed;
            OnRightDragEndEvent -= dragEnd;
        });
    }

    private void RightButtonDown(InputAction.CallbackContext context) => OnDown(1);

    private void RightButtonUp(InputAction.CallbackContext context) => OnUp(1);

    private void LeftDown(InputAction.CallbackContext context) => OnDown(0);

    private void LeftUp(InputAction.CallbackContext context) => OnUp(0);

    private void OnMouseMove(InputAction.CallbackContext context)
    {
        m_prevMousePos = m_mousePos;
        m_mousePos = context.ReadValue<Vector2>();
        var delta = m_mousePos - m_prevMousePos;
        if (m_ActiveButton != -1)
        {
            if (m_DragStarted)
            {
                Debug.Log($"Drag {m_ActiveButton}");
                switch (m_ActiveButton)
                {
                    case 0:
                        OnLeftDragEvent?.Invoke(new MouseDragEventArgs(m_mousePos, delta));
                        break;
                    case 1:
                        OnRightDragEvent?.Invoke(new MouseDragEventArgs(m_mousePos, delta));
                        break;
                }
            }
            else
            {
                m_DragStarted = true;
                Debug.Log($"Drag started {m_ActiveButton}");
                switch (m_ActiveButton)
                {
                    case 0:
                        OnLeftDragStartEvent?.Invoke(new MouseDragEventArgs(m_mousePos, delta));
                        break;
                    case 1:
                        OnRightDragStartEvent?.Invoke(new MouseDragEventArgs(m_mousePos, delta));
                        break;
                }
            }
        }
    }

    private void OnDown(int mouseButton)
    {
        if (m_ActiveButton == -1)
        {
            m_ActiveButton = mouseButton;
        }
    }

    private void OnUp(int mouseButton)
    {
        if (m_ActiveButton == mouseButton)
        {
            if (m_DragStarted)
            {
                m_DragStarted = false;
                Debug.Log($"Drag ended {m_ActiveButton}");
                switch (m_ActiveButton)
                {
                    case 0:
                        OnLeftDragEndEvent?.Invoke(new MouseDragEventArgs(m_mousePos, Vector2.zero));
                        break;
                    case 1:
                        OnRightDragEndEvent?.Invoke(new MouseDragEventArgs(m_mousePos, Vector2.zero));
                        break;
                }
            }

            m_ActiveButton = -1;
        }
    }

    private void OnMouseWheel(InputAction.CallbackContext obj)
    {
        var delta = obj.ReadValue<float>();
        OnWheelEvent?.Invoke(delta);
    }
}