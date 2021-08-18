using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Regicide.Game.EntityMovement
{
    public class PlayerDirectedEntity : NetworkBehaviour
    {
        private PlayerEntityPositionController _playerEntityPositionController = null;
        [SerializeField] private NavMeshAgent _navMeshAgent = null;
        [SerializeField] private bool _testIsMoveable = true;

        [Header("Tracking Settings")]
        [SerializeField] private LayerMask _trackableLayers;
        [SerializeField] private float _minTrackingRange = 1;
        [SerializeField] private float _refreshTrackRate = 0.5f;
        private IEnumerator _trackingCoroutine = null;

        private void Update()
        {
            Keyboard keyboard = InputSystem.GetDevice<Keyboard>();
            if (keyboard.spaceKey.wasPressedThisFrame && _testIsMoveable)
            {
                Debug.Log("Moving " + gameObject.name);
                _playerEntityPositionController.Enable();
            }
        }

        public bool IsTrackingRigidbody { get => _trackingCoroutine != null; }

        public void MoveToPosition(Vector3 targetPosition)
        {
            _navMeshAgent.SetDestination(targetPosition);
        }

        public void TrackRigidbody(Rigidbody rigidbody)
        {
            if (_trackingCoroutine != null)
            {
                StopCoroutine(_trackingCoroutine);
            }
            _trackingCoroutine = TrackRigidbodyUpdate(rigidbody);
            StartCoroutine(_trackingCoroutine);
        }

        private IEnumerator TrackRigidbodyUpdate(Rigidbody rigidbody)
        {
            do
            {
                if (Vector3.Distance(transform.position, rigidbody.position) <= _minTrackingRange)
                {
                    _navMeshAgent.ResetPath();
                    break;
                }
                else
                {
                    MoveToPosition(rigidbody.position);
                }
                yield return new WaitForSeconds(_refreshTrackRate);

            } while (_navMeshAgent.hasPath);
            _trackingCoroutine = null;
        }

        private void Start()
        {
            _playerEntityPositionController = new PlayerEntityPositionController();
            _playerEntityPositionController.PlayerEntityPositioner.MoveToPosition.performed += context =>
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue()), out RaycastHit hit))
                {
                    if (hit.rigidbody != null && _trackableLayers == (_trackableLayers | 1 << hit.rigidbody.gameObject.layer))
                    {
                        TrackRigidbody(hit.rigidbody);
                        Debug.Log("Tracking");
                    }
                    else if (hit.collider != null && _trackableLayers == (_trackableLayers | 1 << hit.collider.gameObject.layer))
                    {
                        MoveToPosition(hit.point);
                        Debug.Log("Move to position");
                    }
                }
                _playerEntityPositionController.Disable();
            };
        }

        private void OnValidate()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _minTrackingRange);
        }
    }
}