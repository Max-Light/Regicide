// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Game/Player/Camera Movement/Player Camera Controller.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Regicide.Game.Player
{
    public class @PlayerCameraController : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @PlayerCameraController()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Player Camera Controller"",
    ""maps"": [
        {
            ""name"": ""Player Camera Movement"",
            ""id"": ""b0169927-7612-423e-9624-1e775bb79504"",
            ""actions"": [
                {
                    ""name"": ""Camera Move"",
                    ""type"": ""Button"",
                    ""id"": ""cfa83cf1-585e-460c-a912-0dad5d4c05b3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera Zoom"",
                    ""type"": ""PassThrough"",
                    ""id"": ""f5f7d6d2-8cf0-4673-b61e-f2bd9d4995f9"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera Drag Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""d36ebfbc-d6e5-4737-b7d7-996d1094a5fa"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Move Vector"",
                    ""id"": ""4bb576f4-ef4b-490a-af25-b3c8b30bd68e"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Camera Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f490ca81-7f91-400b-a87d-90f833eea26f"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard + Mouse"",
                    ""action"": ""Camera Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""a52ecd67-8efb-4892-aa5e-8bb6eb6d3abd"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard + Mouse"",
                    ""action"": ""Camera Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""e338745c-65e6-4ade-a11d-d732a287a2f2"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard + Mouse"",
                    ""action"": ""Camera Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ebda7749-9b6b-4a1b-a6c0-4342863f0022"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard + Mouse"",
                    ""action"": ""Camera Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""744e0083-8758-4a12-b879-c178d4a49276"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Camera Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6eb756a1-7531-407f-a77b-7bb7d14670eb"",
                    ""path"": ""<Pointer>/press"",
                    ""interactions"": ""Hold(duration=0.01)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Camera Drag Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard + Mouse"",
            ""bindingGroup"": ""Keyboard + Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Player Camera Movement
            m_PlayerCameraMovement = asset.FindActionMap("Player Camera Movement", throwIfNotFound: true);
            m_PlayerCameraMovement_CameraMove = m_PlayerCameraMovement.FindAction("Camera Move", throwIfNotFound: true);
            m_PlayerCameraMovement_CameraZoom = m_PlayerCameraMovement.FindAction("Camera Zoom", throwIfNotFound: true);
            m_PlayerCameraMovement_CameraDragMove = m_PlayerCameraMovement.FindAction("Camera Drag Move", throwIfNotFound: true);
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

        // Player Camera Movement
        private readonly InputActionMap m_PlayerCameraMovement;
        private IPlayerCameraMovementActions m_PlayerCameraMovementActionsCallbackInterface;
        private readonly InputAction m_PlayerCameraMovement_CameraMove;
        private readonly InputAction m_PlayerCameraMovement_CameraZoom;
        private readonly InputAction m_PlayerCameraMovement_CameraDragMove;
        public struct PlayerCameraMovementActions
        {
            private @PlayerCameraController m_Wrapper;
            public PlayerCameraMovementActions(@PlayerCameraController wrapper) { m_Wrapper = wrapper; }
            public InputAction @CameraMove => m_Wrapper.m_PlayerCameraMovement_CameraMove;
            public InputAction @CameraZoom => m_Wrapper.m_PlayerCameraMovement_CameraZoom;
            public InputAction @CameraDragMove => m_Wrapper.m_PlayerCameraMovement_CameraDragMove;
            public InputActionMap Get() { return m_Wrapper.m_PlayerCameraMovement; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerCameraMovementActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerCameraMovementActions instance)
            {
                if (m_Wrapper.m_PlayerCameraMovementActionsCallbackInterface != null)
                {
                    @CameraMove.started -= m_Wrapper.m_PlayerCameraMovementActionsCallbackInterface.OnCameraMove;
                    @CameraMove.performed -= m_Wrapper.m_PlayerCameraMovementActionsCallbackInterface.OnCameraMove;
                    @CameraMove.canceled -= m_Wrapper.m_PlayerCameraMovementActionsCallbackInterface.OnCameraMove;
                    @CameraZoom.started -= m_Wrapper.m_PlayerCameraMovementActionsCallbackInterface.OnCameraZoom;
                    @CameraZoom.performed -= m_Wrapper.m_PlayerCameraMovementActionsCallbackInterface.OnCameraZoom;
                    @CameraZoom.canceled -= m_Wrapper.m_PlayerCameraMovementActionsCallbackInterface.OnCameraZoom;
                    @CameraDragMove.started -= m_Wrapper.m_PlayerCameraMovementActionsCallbackInterface.OnCameraDragMove;
                    @CameraDragMove.performed -= m_Wrapper.m_PlayerCameraMovementActionsCallbackInterface.OnCameraDragMove;
                    @CameraDragMove.canceled -= m_Wrapper.m_PlayerCameraMovementActionsCallbackInterface.OnCameraDragMove;
                }
                m_Wrapper.m_PlayerCameraMovementActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @CameraMove.started += instance.OnCameraMove;
                    @CameraMove.performed += instance.OnCameraMove;
                    @CameraMove.canceled += instance.OnCameraMove;
                    @CameraZoom.started += instance.OnCameraZoom;
                    @CameraZoom.performed += instance.OnCameraZoom;
                    @CameraZoom.canceled += instance.OnCameraZoom;
                    @CameraDragMove.started += instance.OnCameraDragMove;
                    @CameraDragMove.performed += instance.OnCameraDragMove;
                    @CameraDragMove.canceled += instance.OnCameraDragMove;
                }
            }
        }
        public PlayerCameraMovementActions @PlayerCameraMovement => new PlayerCameraMovementActions(this);
        private int m_KeyboardMouseSchemeIndex = -1;
        public InputControlScheme KeyboardMouseScheme
        {
            get
            {
                if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard + Mouse");
                return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
            }
        }
        public interface IPlayerCameraMovementActions
        {
            void OnCameraMove(InputAction.CallbackContext context);
            void OnCameraZoom(InputAction.CallbackContext context);
            void OnCameraDragMove(InputAction.CallbackContext context);
        }
    }
}
