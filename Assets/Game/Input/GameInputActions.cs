//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Game/Input/GameInputActions.inputactions
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
                },
                {
                    ""name"": ""LeftButton"",
                    ""type"": ""Button"",
                    ""id"": ""eee14497-fe3b-40a4-aa5f-4850c559ab83"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""RightButton"",
                    ""type"": ""Button"",
                    ""id"": ""2a8d8b92-11d6-4887-90d9-52acc5decaaa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MouseMove"",
                    ""type"": ""Value"",
                    ""id"": ""d0991d00-ebb9-4e56-98d6-b11e8cf02365"",
                    ""expectedControlType"": ""Vector2"",
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
                },
                {
                    ""name"": """",
                    ""id"": ""08ee08bb-c271-456d-a5e1-f020a32e267e"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""018f7bd0-0353-45ed-a442-35949b8a8bdc"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""44168a62-6a61-42c1-b58f-faf6c3c915aa"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightButton"",
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
        m_GameActions_LeftButton = m_GameActions.FindAction("LeftButton", throwIfNotFound: true);
        m_GameActions_RightButton = m_GameActions.FindAction("RightButton", throwIfNotFound: true);
        m_GameActions_MouseMove = m_GameActions.FindAction("MouseMove", throwIfNotFound: true);
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
    private readonly InputAction m_GameActions_LeftButton;
    private readonly InputAction m_GameActions_RightButton;
    private readonly InputAction m_GameActions_MouseMove;
    public struct GameActionsActions
    {
        private @GameInputActions m_Wrapper;
        public GameActionsActions(@GameInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @ScrollAction => m_Wrapper.m_GameActions_ScrollAction;
        public InputAction @LeftButton => m_Wrapper.m_GameActions_LeftButton;
        public InputAction @RightButton => m_Wrapper.m_GameActions_RightButton;
        public InputAction @MouseMove => m_Wrapper.m_GameActions_MouseMove;
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
                @LeftButton.started -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnLeftButton;
                @LeftButton.performed -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnLeftButton;
                @LeftButton.canceled -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnLeftButton;
                @RightButton.started -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnRightButton;
                @RightButton.performed -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnRightButton;
                @RightButton.canceled -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnRightButton;
                @MouseMove.started -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnMouseMove;
                @MouseMove.performed -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnMouseMove;
                @MouseMove.canceled -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnMouseMove;
            }
            m_Wrapper.m_GameActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ScrollAction.started += instance.OnScrollAction;
                @ScrollAction.performed += instance.OnScrollAction;
                @ScrollAction.canceled += instance.OnScrollAction;
                @LeftButton.started += instance.OnLeftButton;
                @LeftButton.performed += instance.OnLeftButton;
                @LeftButton.canceled += instance.OnLeftButton;
                @RightButton.started += instance.OnRightButton;
                @RightButton.performed += instance.OnRightButton;
                @RightButton.canceled += instance.OnRightButton;
                @MouseMove.started += instance.OnMouseMove;
                @MouseMove.performed += instance.OnMouseMove;
                @MouseMove.canceled += instance.OnMouseMove;
            }
        }
    }
    public GameActionsActions @GameActions => new GameActionsActions(this);
    public interface IGameActionsActions
    {
        void OnScrollAction(InputAction.CallbackContext context);
        void OnLeftButton(InputAction.CallbackContext context);
        void OnRightButton(InputAction.CallbackContext context);
        void OnMouseMove(InputAction.CallbackContext context);
    }
}
