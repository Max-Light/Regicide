using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

namespace Regicide.Game.Player
{
    public class PlayerCameraMovementControl : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera = null;
        [SerializeField] private BoxCollider _cameraCollider = null;

        [Header("Movement Commands")]
        [SerializeField] private DirectPositionCameraMovementCommand _directPositionCommand = new DirectPositionCameraMovementCommand();
        [SerializeField] private DragPositionCameraMovementCommand _dragPositionCommand = new DragPositionCameraMovementCommand();
        [SerializeField] private ZoomCameraMovementCommand _zoomCommand = new ZoomCameraMovementCommand();
        [SerializeField] private RotationalCameraMovement _rotateCommand = new RotationalCameraMovement();

        [Header("Camera Control")]
        [SerializeField] private LayerMask _hoverableColliderLayers;

        public CinemachineVirtualCamera VirtualCamera { get => _virtualCamera; }
        public LayerMask HoverableColliderLayers { get => _hoverableColliderLayers; set => _hoverableColliderLayers = value; }

        private IPositionalCameraMovementCommand _positionCommand;
        private BoxCollider _cameraPointerCollider = null;

        private void ScaleCameraPointerCollider()
        {
            LensSettings cameraLens = _virtualCamera.m_Lens;
            float verticalSize = 2 * Mathf.Tan(cameraLens.FieldOfView / 2 * Mathf.PI / 180) * transform.position.y;
            _cameraPointerCollider.size = new Vector3(verticalSize * cameraLens.Aspect, verticalSize, _cameraPointerCollider.size.z);
        }

        private void InitializeDirectMovement(PlayerCameraController cameraController) 
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

        private void InitializeDragMovement(PlayerCameraController cameraController)
        {
            cameraController.PlayerCameraMovement.DirectMove.started += context =>
            {
                _positionCommand = _dragPositionCommand;
                
                _dragPositionCommand.SetOriginDragPoint(Pointer.current.position.ReadValue());
            };
        }

        private void InitializeZoomMovement(PlayerCameraController cameraController)
        {

        }

        private void InitializeRotateMovement(PlayerCameraController cameraController)
        {

        }

        private void Start()
        {
            _positionCommand = _directPositionCommand;
            PlayerCameraController cameraController = GamePlayer.LocalGamePlayer.PlayerCameraController;
            InitializeDirectMovement(cameraController);
            InitializeDragMovement(cameraController);
            InitializeZoomMovement(cameraController);
            InitializeRotateMovement(cameraController);
        }

        private void FixedUpdate()
        {
            _positionCommand.UpdateCameraPosition(this);
            _zoomCommand.UpdateCameraZoom(this);
            _rotateCommand.UpdateCameraRotation(this);
            ScaleCameraPointerCollider();
        }

        private void LateUpdate()
        {
            
        }
    }
}