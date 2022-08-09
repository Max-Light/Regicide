using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Cinemachine;

namespace Regicide.Game.Player
{
    [System.Serializable]
    public class ZoomCameraMovementCommand : IZoomCameraMovementCommand
    {
        [Header("Camera Zoom Movement")]
        [SerializeField] [Min(0)] private float _zoomSpeed = 30;
        private float _scrollDelta = 0;
        private float _targetZoomHeight = 0;

        [Header("Camera Tilt Perspective")]
        [SerializeField] private float _tiltThresholdHeight = 200;
        [SerializeField] private float _maxTiltAngle = 60;
        [SerializeField] private AnimationCurve _tiltAngleInterpolationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        private float _tiltAngle = 0;

        public float ZoomSpeed { get => _zoomSpeed; set => _zoomSpeed = Mathf.Clamp(value, 0, float.MaxValue); }

        public void SetScrollDelta(float delta)
        {
            _scrollDelta = delta;
        }

        public void UpdateCameraZoom(PlayerCameraMovementController cameraControl)
        {
            if (_scrollDelta != 0)
            {
                float deltaZoom = -_scrollDelta * _zoomSpeed;
                float zoomHeight = cameraControl.TargetTransform.position.y - cameraControl.CameraBoundary.min.y;
                _targetZoomHeight = Mathf.Clamp(deltaZoom + zoomHeight, cameraControl.CameraBoundary.min.y, cameraControl.CameraBoundary.max.y);
                Vector3 cameraToPointerOffset = CameraToPointerOffset(cameraControl, zoomHeight, _targetZoomHeight);
                cameraControl.TargetTransform.position += new Vector3(0, _targetZoomHeight - zoomHeight, 0) + cameraToPointerOffset;
            }
            UpdateCameraTilt(cameraControl, cameraControl.TargetTransform.position.y - cameraControl.CameraBoundary.min.y);
        }

        private Vector3 CameraToPointerOffset(PlayerCameraMovementController cameraControl, float zoomHeight, float updatedZoomHeight)
        {
            if (_scrollDelta != 0 && Pointer.current != null)
            {
                Ray ray = cameraControl.PlayerCamera.ScreenPointToRay(Pointer.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
                {
                    Vector3 pointerToCameraOffset = hit.point - cameraControl.TargetTransform.position;
                    pointerToCameraOffset = new Vector3(pointerToCameraOffset.x, 0, pointerToCameraOffset.z);
                    float positionalScale = updatedZoomHeight / zoomHeight;
                    Vector3 calculatedPointerToCameraOffset = pointerToCameraOffset * positionalScale;

                    return new Vector3(pointerToCameraOffset.x - calculatedPointerToCameraOffset.x, 0, pointerToCameraOffset.z - calculatedPointerToCameraOffset.z);
                }
            }
            return Vector3.zero;
        }

        private void UpdateCameraTilt(PlayerCameraMovementController cameraControl, float zoomHeight)
        {
            float updatedTiltAngle = (1 - Mathf.Clamp01(_tiltAngleInterpolationCurve.Evaluate(zoomHeight / _tiltThresholdHeight))) * _maxTiltAngle;
            float deltaTiltAngle = updatedTiltAngle - _tiltAngle;

            float heightFromHoverableCollider;
            Ray ray = cameraControl.PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            {
                heightFromHoverableCollider = cameraControl.TargetTransform.position.y - hit.point.y;
            }
            else
            {
                heightFromHoverableCollider = zoomHeight;
            }
            float positionalOffset = Mathf.Tan(Mathf.Deg2Rad * _tiltAngle) * heightFromHoverableCollider;
            float updatedPositionalOffset = Mathf.Tan(Mathf.Deg2Rad * (_tiltAngle + deltaTiltAngle)) * heightFromHoverableCollider;

            _tiltAngle = updatedTiltAngle;
            cameraControl.TargetTransform.eulerAngles -= new Vector3(deltaTiltAngle, 0, 0);
            cameraControl.TargetTransform.position -= new Vector3(0, 0, updatedPositionalOffset - positionalOffset);
        }
    }
}