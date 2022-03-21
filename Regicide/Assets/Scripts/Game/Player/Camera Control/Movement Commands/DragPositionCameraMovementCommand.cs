using UnityEngine.InputSystem;
using UnityEngine;

namespace Regicide.Game.Player
{
    public class DragPositionCameraMovementCommand : IPositionalCameraMovementCommand
    {
        private Vector2 _originViewportPoint = new Vector2(0.5f, 0.5f);

        public void SetOriginDragPoint(Vector2 dragViewportPoint)
        {
            _originViewportPoint = dragViewportPoint;
        }

        public void UpdateCameraPosition(PlayerCameraMovementController cameraControl)
        {
            Camera camera = cameraControl.PlayerCamera;
            Ray ray;
            RaycastHit hit;

            ray = camera.ViewportPointToRay(_originViewportPoint);
            if (Physics.Raycast(ray, out hit, float.MaxValue, cameraControl.HoverableColliderLayers))
            {
                Vector2 pointerScreenPosition = Pointer.current.position.ReadValue();
                Vector3 dragOriginWorldPoint = new Vector3(hit.point.x, 0, hit.point.z);
                ray = camera.ScreenPointToRay(pointerScreenPosition);
                if (Physics.Raycast(ray, out hit, float.MaxValue, cameraControl.HoverableColliderLayers))
                {
                    Vector3 dragPointerWorldPoint = new Vector3(hit.point.x, 0, hit.point.z);

                    Vector3 dragOffset = dragPointerWorldPoint - dragOriginWorldPoint;

                    cameraControl.TargetTransform.position -= dragOffset;
                    _originViewportPoint = camera.ScreenToViewportPoint(pointerScreenPosition);
                }
            }
        }
    }
}