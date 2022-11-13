//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Game/GameInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @GameInputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @GameInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameInputActions"",
    ""maps"": [
        {
            ""name"": ""GameActions"",
            ""id"": ""b15c5219-0469-417b-ae7e-0c95a40cef89"",
            ""actions"": [
                {
                    ""name"": ""ScrollAction"",
                    ""type"": ""Value"",
                    ""id"": ""f2e87b03-ecac-48ab-b0cc-c14269592232"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6c6aa9a3-a406-4e36-af3e-5a7a76b094f9"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ScrollAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // GameActions
        m_GameActions = asset.FindActionMap("GameActions", throwIfNotFound: true);
        m_GameActions_ScrollAction = m_GameActions.FindAction("ScrollAction", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // GameActions
    private readonly InputActionMap m_GameActions;
    private IGameActionsActions m_GameActionsActionsCallbackInterface;
    private readonly InputAction m_GameActions_ScrollAction;
    public struct GameActionsActions
    {
        private @GameInputActions m_Wrapper;
        public GameActionsActions(@GameInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @ScrollAction => m_Wrapper.m_GameActions_ScrollAction;
        public InputActionMap Get() { return m_Wrapper.m_GameActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameActionsActions set) { return set.Get(); }
        public void SetCallbacks(IGameActionsActions instance)
        {
            if (m_Wrapper.m_GameActionsActionsCallbackInterface != null)
            {
                @ScrollAction.started -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnScrollAction;
                @ScrollAction.performed -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnScrollAction;
                @ScrollAction.canceled -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnScrollAction;
            }
            m_Wrapper.m_GameActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ScrollAction.started += instance.OnScrollAction;
                @ScrollAction.performed += instance.OnScrollAction;
                @ScrollAction.canceled += instance.OnScrollAction;
            }
        }
    }
    public GameActionsActions @GameActions => new GameActionsActions(this);
    public interface IGameActionsActions
    {
        void OnScrollAction(InputAction.CallbackContext context);
    }
}