// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Game/Entity Movement/Player Entity Position Controller.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Regicide.Game.EntityMovement
{
    public class @PlayerEntityPositionController : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @PlayerEntityPositionController()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Player Entity Position Controller"",
    ""maps"": [
        {
            ""name"": ""Player Entity Positioner"",
            ""id"": ""cfedf4fb-08b3-47f9-a07f-8246cb45afdb"",
            ""actions"": [
                {
                    ""name"": ""Move To Position"",
                    ""type"": ""Button"",
                    ""id"": ""679bd6b8-7465-4753-a19d-6b8c8ab614af"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""693337e7-c1c1-4160-89c6-20a6d1ff205b"",
                    ""path"": ""<Pointer>/press"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move To Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Player Entity Positioner
            m_PlayerEntityPositioner = asset.FindActionMap("Player Entity Positioner", throwIfNotFound: true);
            m_PlayerEntityPositioner_MoveToPosition = m_PlayerEntityPositioner.FindAction("Move To Position", throwIfNotFound: true);
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

        // Player Entity Positioner
        private readonly InputActionMap m_PlayerEntityPositioner;
        private IPlayerEntityPositionerActions m_PlayerEntityPositionerActionsCallbackInterface;
        private readonly InputAction m_PlayerEntityPositioner_MoveToPosition;
        public struct PlayerEntityPositionerActions
        {
            private @PlayerEntityPositionController m_Wrapper;
            public PlayerEntityPositionerActions(@PlayerEntityPositionController wrapper) { m_Wrapper = wrapper; }
            public InputAction @MoveToPosition => m_Wrapper.m_PlayerEntityPositioner_MoveToPosition;
            public InputActionMap Get() { return m_Wrapper.m_PlayerEntityPositioner; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerEntityPositionerActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerEntityPositionerActions instance)
            {
                if (m_Wrapper.m_PlayerEntityPositionerActionsCallbackInterface != null)
                {
                    @MoveToPosition.started -= m_Wrapper.m_PlayerEntityPositionerActionsCallbackInterface.OnMoveToPosition;
                    @MoveToPosition.performed -= m_Wrapper.m_PlayerEntityPositionerActionsCallbackInterface.OnMoveToPosition;
                    @MoveToPosition.canceled -= m_Wrapper.m_PlayerEntityPositionerActionsCallbackInterface.OnMoveToPosition;
                }
                m_Wrapper.m_PlayerEntityPositionerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @MoveToPosition.started += instance.OnMoveToPosition;
                    @MoveToPosition.performed += instance.OnMoveToPosition;
                    @MoveToPosition.canceled += instance.OnMoveToPosition;
                }
            }
        }
        public PlayerEntityPositionerActions @PlayerEntityPositioner => new PlayerEntityPositionerActions(this);
        public interface IPlayerEntityPositionerActions
        {
            void OnMoveToPosition(InputAction.CallbackContext context);
        }
    }
}
