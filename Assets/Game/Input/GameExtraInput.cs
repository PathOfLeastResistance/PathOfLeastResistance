using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityTools;

public class GameExtraInput : SingletonMonobehaviour<GameExtraInput>
{
    private GameInputActions m_Actions;

    public event Action<float> OnWheelEvent;
    
    void OnEnable()
    {
        m_Actions = new GameInputActions();
        m_Actions.GameActions.Enable();

        m_Actions.GameActions.ScrollAction.performed += OnPointerEvent;
        m_Actions.GameActions.ScrollAction.canceled += OnPointerEvent;
    }

    private void OnPointerEvent(InputAction.CallbackContext obj)
    {
        var delta = obj.ReadValue<float>();
        OnWheelEvent?.Invoke(delta);
    }
}
