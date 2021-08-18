
using Cinemachine;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Regicide.Game.Player
{
    public class PlayerCameraMovement : NetworkBehaviour
    {
        private PlayerCameraController _controller = null;
        private Collider _boundingCollider = null;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera = null;

        [Header("Camera Movement")]
        [SerializeField] [Min(0)] private float _cameraMovementSpeed = 1;

        [Header("Camera Zoom")]
        [SerializeField] [Min(0)] private float _cameraZoomIncrement = 1;
        [SerializeField] [Min(0)] private float _cameraZoomingSpeed = 1;

        [Header("Orthographic Mode")]
        [SerializeField] [Min(0)] private float _minOrthographicSize = 3;
        [SerializeField] [Min(0)] private float _maxOrthographicSize = 10;

        [Header("Hoverable Camera Settings")]
        [SerializeField] private LayerMask _hoverableColliderLayers;

        private Vector2 _currentCameraMoveDirection = Vector2.zero;
        private Vector2 _dragPointerOrigin = Vector2.zero;
        private Vector2 _dragCameraOriginWorldPosition = Vector2.zero;
        private bool _isDraggingCamera = false;
        private bool _isMovingCamera = false;
        private float _targetOrthographicSize = 0;
        private float _minTargetCameraHeight = 0;

        private ICommand _cameraMovementCommand = null;
        private ICommand _cameraDragMovementCommand = null;
        private ICommand _orthographicCameraZoomCommand = null;
        private ICommand _perspectiveCameraZoomCommand = null;
 
        public CinemachineVirtualCamera CinemachineVirtualCamera { get => _virtualCamera; }
        public float CameraMovementSpeed { get => _cameraMovementSpeed; set => _cameraMovementSpeed = Mathf.Clamp(value, 0, float.MaxValue); }
        public float CameraZoomIncrementalSpeed { get => _cameraZoomIncrement; set => _cameraZoomIncrement = Mathf.Clamp(value, 0, float.MaxValue); }
        public float CameraZoomingSpeed { get => _cameraZoomingSpeed; set => _cameraZoomingSpeed = Mathf.Clamp(value, 0, float.MaxValue); }
        public float MinOrthographicSize { get => _minOrthographicSize; }
        public float MaxOrthographicSize { get => _maxOrthographicSize; }
        public Vector2 CurrentMoveDirection { get => _currentCameraMoveDirection; }
        public Vector2 DragPointerOriginPosition { get => _dragPointerOrigin; }
        public Vector2 DragCameraOriginalWorldPosition { get => _dragCameraOriginWorldPosition; }
        public bool IsMovingCamera { get => _isMovingCamera; }
        public float TargetOrthographicSize { get => _targetOrthographicSize; }
        public float TargetCameraHeight { get => CalculateTargetCameraHeight(); }
        public Vector2 CameraWorldUnitResolution { get => CalculateCameraWorldUnitResolution(); }

        private void ActivateVirtualCameraEnvironment()
        {
            _virtualCamera.gameObject.SetActive(true);
            Collider boundingCollider = PlayerCameraEnvironment.CameraEnvironmentConfiner;
            if (boundingCollider != null)
            {
                _virtualCamera.GetComponent<CinemachineConfiner>().m_BoundingVolume = boundingCollider;
                _boundingCollider = boundingCollider;
                PlayerCameraEnvironment.ConfineTransform(transform, CalculateCameraWorldUnitResolution());
                enabled = true;
            }
            else
            {
                Debug.LogError("No bounding collider detected");
                enabled = false;
            }
        }

        private void ActivatePlayerCameraControlScheme()
        {
            if (_controller == null)
            {
                _controller = new PlayerCameraController();
            }
            _controller.Enable();

            _cameraMovementCommand = new CameraMovementCommand(this);
            _orthographicCameraZoomCommand = new OrthographicCameraZoomCommand(this);
            _perspectiveCameraZoomCommand = new PerspectiveCameraZoomCommand(this);
            _cameraDragMovementCommand = new CameraDragMovementCommand(this);

            _controller.PlayerCameraMovement.CameraMove.performed += context =>
            {
                _controller.PlayerCameraMovement.CameraDragMove.Disable();
                _currentCameraMoveDirection = context.ReadValue<Vector2>().normalized;
                _isMovingCamera = true;
            };
            _controller.PlayerCameraMovement.CameraMove.canceled += context =>
            {
                _controller.PlayerCameraMovement.CameraDragMove.Enable();
                _currentCameraMoveDirection = Vector2.zero;
                _isMovingCamera = false;
            };
            _controller.PlayerCameraMovement.CameraDragMove.performed += context =>
            {
                _controller.PlayerCameraMovement.CameraMove.Disable();
                _controller.PlayerCameraMovement.CameraZoom.Disable();
                _dragPointerOrigin = Pointer.current.position.ReadValue();
                _dragCameraOriginWorldPosition = new Vector2(transform.position.x, transform.position.z);
                _isDraggingCamera = true;
                _isMovingCamera = true;
            };
            _controller.PlayerCameraMovement.CameraDragMove.canceled += context =>
            {
                _controller.PlayerCameraMovement.CameraMove.Enable();
                _controller.PlayerCameraMovement.CameraZoom.Enable();
                _dragPointerOrigin = Vector2.zero;
                _dragCameraOriginWorldPosition = Vector2.zero;
                _isDraggingCamera = false;
                _isMovingCamera = false;
            };
            _controller.PlayerCameraMovement.CameraZoom.performed += context =>
            {
                float scrollDelta = Mathf.Clamp(context.ReadValue<float>(), -1, 1);
                if (scrollDelta != 0)
                {
                    if (_virtualCamera.m_Lens.Orthographic)
                    {
                        _targetOrthographicSize = Mathf.Clamp(_cameraZoomIncrement * -scrollDelta + _targetOrthographicSize, _minOrthographicSize, _maxOrthographicSize);
                    }
                    else
                    {
                        _minTargetCameraHeight = Mathf.Clamp(_cameraZoomIncrement * -scrollDelta + _minTargetCameraHeight, _boundingCollider.bounds.min.y, _boundingCollider.bounds.max.y);
                    }
                }
            };
        }

        private float CalculateTargetCameraHeight()
        {
            Vector3 targetPosition = new Vector3(transform.position.x, _minTargetCameraHeight, transform.position.z);
            float minYHeight = _boundingCollider.bounds.min.y;
            if (Physics.Raycast(targetPosition, Vector3.down, out RaycastHit hit, minYHeight, _hoverableColliderLayers))
            {
                return hit.point.y + minYHeight;
            }
            return _minTargetCameraHeight;
        }

        private Vector2 CalculateCameraWorldUnitResolution()
        {
            LensSettings virtualCameraLens = _virtualCamera.m_Lens;
            if (virtualCameraLens.Orthographic)
            {
                return new Vector2(virtualCameraLens.OrthographicSize * virtualCameraLens.Aspect * 2, virtualCameraLens.OrthographicSize * 2);
            }
            else
            {
                float verticalSize = 2 * Mathf.Tan(virtualCameraLens.FieldOfView / 2 * Mathf.PI / 180) * transform.position.y;
                return new Vector2(verticalSize * virtualCameraLens.Aspect, verticalSize);
            }
        }

        private void UpdateCameraMovement()
        {
            if (_isDraggingCamera)
            {
                _cameraDragMovementCommand.Execute();
            }
            else
            {
                _cameraMovementCommand.Execute();
            }
        }

        private void UpdateCameraZoom()
        {
            if (_virtualCamera.m_Lens.Orthographic)
            {
                _orthographicCameraZoomCommand.Execute();
            }
            else
            {
                _perspectiveCameraZoomCommand.Execute();
            }

            Vector3 targetPosition = new Vector3(transform.position.x, _minTargetCameraHeight, transform.position.z);
            float minYHeight = _boundingCollider.bounds.min.y;
            if (Physics.Raycast(targetPosition, Vector3.down, out RaycastHit hit, minYHeight, _hoverableColliderLayers))
            {
                float targetHeight = hit.point.y + minYHeight;
                _minTargetCameraHeight = targetHeight - (targetHeight % _cameraZoomIncrement);
            }
        }

        private void FixedUpdate()
        {
            UpdateCameraMovement();
            UpdateCameraZoom();
            PlayerCameraEnvironment.ConfineTransform(transform, CalculateCameraWorldUnitResolution());
        }

        private void Awake()
        {
            enabled = false;
            _virtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(_virtualCamera.m_Lens.OrthographicSize, _minOrthographicSize, _maxOrthographicSize);
            _targetOrthographicSize = _virtualCamera.m_Lens.OrthographicSize;
            _minTargetCameraHeight = transform.position.y;
        }

        public override void OnStartClient()
        {
            if (!hasAuthority)
            {
                Destroy(_virtualCamera.gameObject);
                Destroy(this);
            }
        }

        public override void OnStartLocalPlayer()
        {
            ActivateVirtualCameraEnvironment();
            ActivatePlayerCameraControlScheme();
        }

        private void OnDestroy()
        {
            if (_controller != null && hasAuthority)
            {
                _controller.Disable();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (PlayerCameraEnvironment.CameraEnvironmentConfiner != null)
            {
                Gizmos.color = Color.white;
                Vector3 targetPosition = new Vector3(transform.position.x, _minTargetCameraHeight, transform.position.z);
                Gizmos.DrawRay(targetPosition, Vector3.down * PlayerCameraEnvironment.CameraEnvironmentConfiner.bounds.min.y);
            }
        }
    }
}