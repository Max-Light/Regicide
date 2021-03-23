
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

        private CameraMovementCommand moveCommand = null;
        private CameraZoomCommand zoomCommand = null;
 
        private void OnValidate()
        {
            playerRigidbody = GetComponent<Rigidbody2D>();
            cameraCollider = GetComponent<BoxCollider2D>();
        }

        private void Awake()
        {
            controller = new PlayerCameraController();
            moveCommand = new CameraMovementCommand();
            zoomCommand = new CameraZoomCommand(cinemachineCamera.m_Lens.OrthographicSize);
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
            controller.PlayerCameraMovement.CameraMove.performed += context => moveCommand.Execute(playerRigidbody, context.ReadValue<Vector2>().normalized, movementSpeed);
            controller.PlayerCameraMovement.CameraMove.canceled += context => moveCommand.Execute(playerRigidbody, Vector2.zero, 0);
            controller.PlayerCameraMovement.CameraZoom.performed += context => zoomCommand.Execute(cinemachineCamera, Mathf.Clamp(context.ReadValue<float>(), -1, 1), zoomIncrement);
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
            zoomCommand.UpdateOrhtoGrahpicSizeWithZoomMovement(playerRigidbody, cinemachineCamera, cameraCollider, zoomSpeed);
        }
    }
}