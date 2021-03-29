
using Cinemachine;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Regicide.Game.Player
{
    public class PlayerCameraMovement : NetworkBehaviour
    {
        private PlayerCameraController controller = null;
        [SerializeField] private CinemachineVirtualCamera cinemachineCamera = null;
        [SerializeField] private Rigidbody2D playerRigidbody = null;
        [SerializeField] private BoxCollider2D cameraCollider = null;
        [SerializeField] private float movementSpeed = 1;
        [SerializeField] private float zoomIncrement = 1;
        [SerializeField] private float zoomSpeed = 1;
        [SerializeField] private float minOrthographicSize = 3;
        [SerializeField] private float maxOrthographicSize = 10;
        private float targetOrthographicSize = 0;

        public float TargetOrthographicSize
        {
            get => targetOrthographicSize;
            set
            {
                targetOrthographicSize = Mathf.Clamp(value, minOrthographicSize, maxOrthographicSize);
            }
        }

        private void OnValidate()
        {
            playerRigidbody = GetComponent<Rigidbody2D>();
            cameraCollider = GetComponent<BoxCollider2D>();
        }

        private void Awake()
        {
            controller = new PlayerCameraController();
            targetOrthographicSize = cinemachineCamera.m_Lens.OrthographicSize;
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            ActivateCinemachineCamera();
            ActivateCameraCollider();
            ActivatePlayerCameraControlScheme();
        }

        public override void OnStopAuthority()
        {
            base.OnStopAuthority();
            controller.Disable();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!hasAuthority)
            {
                enabled = false;
            }
        }

        private void ActivatePlayerCameraControlScheme()
        {
            controller.Enable();
            controller.PlayerCameraMovement.CameraMove.performed += context =>
            {
                ICommand moveCommand = new CameraMovementCommand(playerRigidbody, context.ReadValue<Vector2>().normalized, movementSpeed);
                moveCommand.Execute();
            };
            controller.PlayerCameraMovement.CameraMove.canceled += context =>
            {
                ICommand moveCommand = new CameraMovementCommand(playerRigidbody, Vector2.zero, 0);
                moveCommand.Execute();
            };
            controller.PlayerCameraMovement.CameraZoom.performed += context =>
            {
                ICommand newTargetOrthographicSizeCommand = new CameraSetTargetOrthographicSizeCommand(this, Mathf.Clamp(context.ReadValue<float>(), -1, 1), zoomIncrement);
                newTargetOrthographicSizeCommand.Execute();
            };
        }

        private void ActivateCinemachineCamera()
        {
            cinemachineCamera.gameObject.SetActive(true);
            if (PlayerCameraEnvironment.Singleton != null)
            {
                cinemachineCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = PlayerCameraEnvironment.Singleton.VirtualCameraConfinerCollider;
            }
            else
            {
                Debug.LogError("Camera environment could not be found!");
            }
        }

        private void ActivateCameraCollider()
        {
            cameraCollider.enabled = true;
            LensSettings virtualCameraLens = cinemachineCamera.m_Lens;
            cameraCollider.size = new Vector2(virtualCameraLens.OrthographicSize * virtualCameraLens.Aspect * 2, virtualCameraLens.OrthographicSize * 2);
        }

        private void FixedUpdate()
        {
            ICommand cameraZoomCommand = new CameraZoomCommand(playerRigidbody, cinemachineCamera, cameraCollider, targetOrthographicSize, zoomSpeed);
            cameraZoomCommand.Execute();
        }
    }
}