
using Cinemachine;
using Mirror;
using UnityEngine;

namespace Regicide.Game.Player
{
    public class PlayerCameraMovement : NetworkBehaviour
    {
        private PlayerCameraController _controller = null;
        [SerializeField] private CinemachineVirtualCamera _cinemachineCamera = null;
        [SerializeField] private Rigidbody _playerRigidbody = null;
        [SerializeField] private BoxCollider _cameraCollider = null;
        [SerializeField] private float _movementSpeed = 1;
        [SerializeField] private float _zoomIncrement = 1;
        [SerializeField] private float _zoomSpeed = 1;
        [SerializeField] private float _minOrthographicSize = 3;
        [SerializeField] private float _maxOrthographicSize = 10;
        private float _targetOrthographicSize = 0;

        public float TargetOrthographicSize
        {
            get => _targetOrthographicSize;
            set
            {
                _targetOrthographicSize = Mathf.Clamp(value, _minOrthographicSize, _maxOrthographicSize);
            }
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
            _controller.Disable();
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
            _controller.Enable();
            _controller.PlayerCameraMovement.CameraMove.performed += context =>
            {
                ICommand moveCommand = new CameraMovementCommand(_playerRigidbody, context.ReadValue<Vector2>().normalized, _movementSpeed);
                moveCommand.Execute();
            };
            _controller.PlayerCameraMovement.CameraMove.canceled += context =>
            {
                ICommand moveCommand = new CameraMovementCommand(_playerRigidbody, Vector2.zero, 0);
                moveCommand.Execute();
            };
            _controller.PlayerCameraMovement.CameraZoom.performed += context =>
            {
                ICommand newTargetOrthographicSizeCommand = new CameraSetTargetOrthographicSizeCommand(this, Mathf.Clamp(context.ReadValue<float>(), -1, 1), _zoomIncrement);
                newTargetOrthographicSizeCommand.Execute();
            };
        }

        private void ActivateCinemachineCamera()
        {
            _cinemachineCamera.gameObject.SetActive(true);
            _cinemachineCamera.GetComponent<CinemachineConfiner>().m_BoundingVolume = PlayerCameraEnvironment.CameraEnvironmentConfiner;
        }

        private void ActivateCameraCollider()
        {
            _cameraCollider.enabled = true;
            LensSettings virtualCameraLens = _cinemachineCamera.m_Lens;
            _cameraCollider.size = new Vector2(virtualCameraLens.OrthographicSize * virtualCameraLens.Aspect * 2, virtualCameraLens.OrthographicSize * 2);
        }

        private void OnValidate()
        {
            _playerRigidbody = GetComponent<Rigidbody>();
            _cameraCollider = GetComponent<BoxCollider>();
        }

        private void Awake()
        {
            _controller = new PlayerCameraController();
            _targetOrthographicSize = _cinemachineCamera.m_Lens.OrthographicSize;
        }

        private void FixedUpdate()
        {
            ICommand cameraZoomCommand = new CameraZoomCommand(_playerRigidbody, _cinemachineCamera, _cameraCollider, _targetOrthographicSize, _zoomSpeed);
            cameraZoomCommand.Execute();
        }
    }
}