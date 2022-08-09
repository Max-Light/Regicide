using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine;
using System;

namespace Regicide.Game.Player
{
    public class PlayerCameraMovementController : MonoBehaviour
    {
        [SerializeField] private Camera _camera = null;
        [SerializeField] private Transform _targetTransform = null;

        [Header("Movement Commands")]
        [SerializeField] private DirectPositionCameraMovementCommand _directPositionCommand = new DirectPositionCameraMovementCommand();
        [SerializeField] private DragPositionCameraMovementCommand _dragPositionCommand = new DragPositionCameraMovementCommand();
        [SerializeField] private ZoomCameraMovementCommand _zoomCommand = new ZoomCameraMovementCommand();
        [SerializeField] private RotateCameraMovementCommand _rotateCommand = new RotateCameraMovementCommand();

        [Header("Camera Control")]
        [SerializeField] private float _hoverDistance = 35;
        [SerializeField] private Bounds _cameraBoundary;

        private IPositionalCameraMovementCommand _positionCommand;

        public Camera PlayerCamera { get => _camera; }
        public Transform TargetTransform { get => _targetTransform; }
        public Bounds CameraBoundary { get => _cameraBoundary; set => _cameraBoundary = value; }

        private void InitializeDirectMovement(PlayerInputController cameraController) 
        {
            cameraController.PlayerCameraMovement.DirectMove.started += context =>
            {
                _positionCommand = _directPositionCommand;
            };
            cameraController.PlayerCameraMovement.DirectMove.performed += context =>
            {
                _directPositionCommand.SetDirection(context.ReadValue<Vector2>().normalized);
            };
            cameraController.PlayerCameraMovement.DirectMove.canceled += context =>
            {
                _directPositionCommand.SetDirection(Vector3.zero);
            };
        }

        private void InitializeDragMovement(PlayerInputController cameraController)
        {
            cameraController.PlayerCameraMovement.DragMove.started += context =>
            {
                _positionCommand = _dragPositionCommand;
                _dragPositionCommand.SetOriginDragPoint(PlayerCamera.ScreenToViewportPoint(Pointer.current.position.ReadValue()));
            };
            cameraController.PlayerCameraMovement.DragMove.canceled += context =>
            {
                _positionCommand = _directPositionCommand;
            };
        }

        private void InitializeZoomMovement(PlayerInputController cameraController)
        {
            cameraController.PlayerCameraMovement.Zoom.performed += context =>
            {
                _zoomCommand.SetScrollDelta(Mathf.Clamp(context.ReadValue<float>(), -1, 1));
            };
            _zoomCommand.UpdateCameraZoom(this);
        }

        private void InitializeRotateMovement(PlayerInputController cameraController)
        {
            cameraController.PlayerCameraMovement.Rotate.performed += context =>
            {
                _rotateCommand.RotationalDirection = context.ReadValue<float>();
            };
            cameraController.PlayerCameraMovement.Rotate.canceled += context =>
            {
                _rotateCommand.RotationalDirection = 0;
            };
        }

        private void ConfineCamera()
        {
            Vector3 cameraPosition = _targetTransform.position;

            float cameraHeight = cameraPosition.y - _cameraBoundary.min.y;
            float cameraHeightWorldUnit = Mathf.Tan(Mathf.Deg2Rad * (PlayerCamera.fieldOfView / 2)) * cameraHeight * 2;
            Vector2 cameraWorldUnitResolution = new Vector2(cameraHeightWorldUnit * PlayerCamera.aspect, cameraHeightWorldUnit);

            float cameraRotateAngle = Mathf.Deg2Rad * _targetTransform.eulerAngles.y % 90;
            float side1 = Mathf.Abs(Mathf.Sin(cameraRotateAngle) * cameraWorldUnitResolution.x);
            float side2 = Mathf.Abs(Mathf.Cos(cameraRotateAngle) * cameraWorldUnitResolution.x);
            float side3 = Mathf.Abs(Mathf.Sin(cameraRotateAngle) * cameraWorldUnitResolution.y);
            float side4 = Mathf.Abs(Mathf.Cos(cameraRotateAngle) * cameraWorldUnitResolution.y);

            Vector2 cameraBoundSize = new Vector2(side2 + side3, side1 + side4);
            Vector2 cameraHalfBoundSize = cameraBoundSize / 2;

            float cameraTiltAngle = Mathf.Deg2Rad * (90 - _targetTransform.eulerAngles.x);
            float offsetMagnitude = Mathf.Tan(cameraTiltAngle) * cameraHeight;
            float xOffset = -Mathf.Sin(cameraRotateAngle) * offsetMagnitude;
            float zOffset = -Mathf.Cos(cameraRotateAngle) * offsetMagnitude;

            float bottomBound = _cameraBoundary.min.y;
            if (Physics.Raycast(_targetTransform.position + (-_targetTransform.forward * (_cameraBoundary.max.y - _cameraBoundary.min.y)), _targetTransform.forward, out RaycastHit hit, float.MaxValue))
            {
                bottomBound = hit.point.y + _hoverDistance;
            }

            cameraPosition = new Vector3
            {
                x = Mathf.Clamp(cameraPosition.x, _cameraBoundary.min.x + cameraHalfBoundSize.x + xOffset, _cameraBoundary.max.x - cameraHalfBoundSize.x + xOffset),
                y = Mathf.Clamp(cameraPosition.y, bottomBound, _cameraBoundary.max.y),
                z = Mathf.Clamp(cameraPosition.z, _cameraBoundary.min.z + cameraHalfBoundSize.y + zOffset, _cameraBoundary.max.z - cameraHalfBoundSize.y + zOffset)
            };
            _targetTransform.position = cameraPosition;
        }

        private void Start()
        {
            _positionCommand = _directPositionCommand;
            PlayerInputController cameraController = GamePlayer.LocalGamePlayer.PlayerInputControl;
            InitializeDirectMovement(cameraController);
            InitializeDragMovement(cameraController);
            InitializeZoomMovement(cameraController);
            InitializeRotateMovement(cameraController);
        }

        private void Update()
        {
            _positionCommand.UpdateCameraPosition(this);
            _zoomCommand.UpdateCameraZoom(this);
            _rotateCommand.UpdateCameraRotation(this);
            ConfineCamera();
        }

        private void OnValidate()
        {
            if (PlayerCamera != null)
            {
                float minBoundLength = Mathf.Min((_cameraBoundary.max.x - _cameraBoundary.min.x) / 2, (_cameraBoundary.max.z - _cameraBoundary.min.z) / 2);
                float maxRadius = Mathf.Sqrt(Mathf.Pow(minBoundLength, 2) / (Mathf.Pow(PlayerCamera.aspect, 2) + 1));
                _cameraBoundary.max = new Vector3(_cameraBoundary.max.x, Mathf.Clamp(_cameraBoundary.max.y, _cameraBoundary.min.y, (maxRadius / Mathf.Tan(Mathf.Deg2Rad * (PlayerCamera.fieldOfView / 2))) + _cameraBoundary.min.y), _cameraBoundary.max.z);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_targetTransform != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(_targetTransform.position, _targetTransform.forward * 400);
            }
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_cameraBoundary.center, _cameraBoundary.extents * 2);
        }
    }
}