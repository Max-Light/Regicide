using Mirror;
using System.Collections;
using Unity.AI.Navigation.Editor;
using UnityEditor;
using UnityEditor.AI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using System.Threading;

namespace Regicide.Game.Entity.Navigation
{
    [CustomEditor(typeof(PlayerNavigatedAgent))]
    [CanEditMultipleObjects]
    public class PlayerNavigatedAgentEditor: Editor
    {
        protected SerializedProperty _areaMask = null;
        protected SerializedProperty _agentTypeId = null;

        protected SerializedProperty _hasTrackAbility = null;
        protected SerializedProperty _trackRefreshRate = null;

        protected SerializedProperty _basePositionalSpeed = null;
        protected SerializedProperty _terminalPositionalSpeed = null;
        protected SerializedProperty _positionalAcceleration = null;
        protected SerializedProperty _isSmoothPositionalStoppingEnabled = null;
        protected SerializedProperty _stoppingBoundaryDistance = null;
        protected SerializedProperty _positionalDeacceleration = null;
        protected SerializedProperty _turningAngleTolerance = null;

        protected SerializedProperty _baseAngularSpeed = null;
        protected SerializedProperty _terminalAngularSpeed = null;
        protected SerializedProperty _angularAcceleration = null;
        protected SerializedProperty _angularDeacceleration = null;
        protected SerializedProperty _stoppingAngle = null;
        protected SerializedProperty _turnBoundaryDistance = null;

        protected SerializedProperty _alignmentLayers = null;
        protected SerializedProperty _alignmentRotationalSpeed = null;

        private void OnEnable()
        {
            _areaMask = serializedObject.FindProperty(nameof(_areaMask));
            _agentTypeId = serializedObject.FindProperty(nameof(_agentTypeId));

            _hasTrackAbility = serializedObject.FindProperty(nameof(_hasTrackAbility));
            _trackRefreshRate = serializedObject.FindProperty(nameof(_trackRefreshRate));

            _basePositionalSpeed = serializedObject.FindProperty(nameof(_basePositionalSpeed));
            _terminalPositionalSpeed = serializedObject.FindProperty(nameof(_terminalPositionalSpeed));
            _positionalAcceleration = serializedObject.FindProperty(nameof(_positionalAcceleration));
            _positionalDeacceleration = serializedObject.FindProperty(nameof(_positionalDeacceleration));
            _isSmoothPositionalStoppingEnabled = serializedObject.FindProperty(nameof(_isSmoothPositionalStoppingEnabled));
            _stoppingBoundaryDistance = serializedObject.FindProperty(nameof(_stoppingBoundaryDistance));
            _turningAngleTolerance = serializedObject.FindProperty(nameof(_turningAngleTolerance));

            _baseAngularSpeed = serializedObject.FindProperty(nameof(_baseAngularSpeed));
            _terminalAngularSpeed = serializedObject.FindProperty(nameof(_terminalAngularSpeed));
            _angularAcceleration = serializedObject.FindProperty(nameof(_angularAcceleration));
            _angularDeacceleration = serializedObject.FindProperty(nameof(_angularDeacceleration));
            _turnBoundaryDistance = serializedObject.FindProperty(nameof(_turnBoundaryDistance));
            _stoppingAngle = serializedObject.FindProperty(nameof(_stoppingAngle));

            _alignmentLayers = serializedObject.FindProperty(nameof(_alignmentLayers));
            _alignmentRotationalSpeed = serializedObject.FindProperty(nameof(_alignmentRotationalSpeed));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            AreaPopup("Area Mask", _areaMask);
            NavMeshComponentsGUIUtility.AgentTypePopup("Agent Type ID", _agentTypeId);

            EditorGUILayout.PropertyField(_hasTrackAbility);
            EditorGUILayout.PropertyField(_trackRefreshRate);

            EditorGUILayout.PropertyField(_basePositionalSpeed);
            EditorGUILayout.PropertyField(_terminalPositionalSpeed);
            EditorGUILayout.PropertyField(_positionalAcceleration);
            EditorGUILayout.PropertyField(_positionalDeacceleration);
            EditorGUILayout.PropertyField(_isSmoothPositionalStoppingEnabled);
            EditorGUILayout.PropertyField(_stoppingBoundaryDistance);
            EditorGUILayout.PropertyField(_turningAngleTolerance);

            EditorGUILayout.PropertyField(_baseAngularSpeed);
            EditorGUILayout.PropertyField(_terminalAngularSpeed);
            EditorGUILayout.PropertyField(_angularAcceleration);
            EditorGUILayout.PropertyField(_angularDeacceleration);
            EditorGUILayout.PropertyField(_turnBoundaryDistance);
            EditorGUILayout.PropertyField(_stoppingAngle);

            EditorGUILayout.PropertyField(_alignmentLayers);
            EditorGUILayout.PropertyField(_alignmentRotationalSpeed);

            serializedObject.ApplyModifiedProperties();
        }

        public static void AreaPopup(string labelName, SerializedProperty areaProperty)
        {
            string[] areaNames = GameObjectUtility.GetNavMeshAreaNames();
            int bitMask = areaProperty.intValue;

            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            EditorGUI.BeginProperty(rect, GUIContent.none, areaProperty);

            EditorGUI.BeginChangeCheck();
            bitMask = EditorGUI.MaskField(rect, labelName, bitMask, areaNames);

            if (EditorGUI.EndChangeCheck())
            {
                if (bitMask == (int)Mathf.Pow(2, areaNames.Length) - 1)
                {
                    areaProperty.intValue = NavMesh.AllAreas;
                }
                areaProperty.intValue = bitMask;
            }

            EditorGUI.EndProperty();
        }
    }

    public class PlayerNavigatedAgent : NetworkBehaviour
    {
        //<summary>
        //NOTE: A bug occurs in Unity that creates invalid paths near nav mesh obstacles. The solution is to decrease the voxel size of the nav mesh surface manually.
        //</summary>

        [Header("General")]
        [SerializeField] private int _areaMask = NavMesh.AllAreas;
        [SerializeField] private int _agentTypeId = 0;
        
        [SerializeField] private bool _testIsMoveable = true;
        private PlayerEntityPositionController _playerEntityPositionController = null;
        private NavMeshPath _navMeshPath = null;
        private Vector3[] _path = new Vector3[0];
        private Vector3 _lookDirection = Vector3.zero;

        [Header("Tracking")]
        [SerializeField] private bool _hasTrackAbility = true;
        [SerializeField] private float _trackRefreshRate = 0.5f;
        private IEnumerator _trackingCoroutine = null;
        private Transform _trackedTransform = null;

        [Header("Positional Steering")]
        [Tooltip("The base positional speed amount when starting and ending a path.")]
        [SerializeField] [Min(0)] protected float _basePositionalSpeed = 0;
        [Tooltip("The maximum positional speed.")]
        [SerializeField] [Min(0.01f)] protected float _terminalPositionalSpeed = 1;
        [SerializeField] [Min(0.01f)] protected float _positionalAcceleration = 1;
        [Tooltip("The default positional deacceleration of the object.")]
        [SerializeField] [Min(0.01f)] protected float _positionalDeacceleration = 1;
        [Tooltip("Enable the smooth deaccelerate positional speed feature when coming to a stop.")]
        [SerializeField] protected bool _isSmoothPositionalStoppingEnabled = true;
        [Tooltip("The trigger distance from the path's last point to deaccelerate on the positional vector. Note: The deacceleration is based on scale from the distance remaining with the defined stopping boundary distance.")]
        [SerializeField] [Min(0)] protected float _stoppingBoundaryDistance = 3;
        [Tooltip("The maximum tolerable angle for the object to keep a target positional speed above the base speed. Target positional speed is scaled based on the remaining turn angle and the defined tolerable angle.")]
        [SerializeField] [Range(0, 180)] protected float _turningAngleTolerance = 90;

        [Header("Angular Steering")]
        [Tooltip("The base angular speed amount when starting and stopping to rotate.")]
        [SerializeField] [Min(0)] protected float _baseAngularSpeed = 0;
        [Tooltip("The maximum angular speed.")]
        [SerializeField] [Min(0.01f)] protected float _terminalAngularSpeed = 15;
        [SerializeField] [Min(0.01f)] protected float _angularAcceleration = 5;
        [Tooltip("The default angular deacceleration of the object.")]
        [SerializeField] [Min(0.01f)] protected float _angularDeacceleration = 5;
        [Tooltip("The distance before a waypoint to trigger the turning event.")]
        [SerializeField] protected float _turnBoundaryDistance = 0;
        [Tooltip("The trigger degree left when turning to deaccelerate on the rotational vector. Note: The deacceleration is based on a scale from the amount of degree left when turning and the defined stopping angle.")]
        [SerializeField] [Range(0, 180)] protected float _stoppingAngle = 0;

        [Header("Alignment")]
        [SerializeField] private LayerMask _alignmentLayers;
        [SerializeField] [Min(0.01f)] private float _alignmentRotationalSpeed = 25;

        protected float _positionalSpeed = 0;
        protected float _angularSpeed = 0;
        private Vector3 _turnDirection = Vector3.zero;
        private float _faceForwardDegrees = 0;

        private Object _lock = new Object();

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame && _testIsMoveable)
            {
                _playerEntityPositionController.Enable();
            }
        }

        public int AreaMask { get => _areaMask; set => _areaMask = value; }
        public int AgentTypeID { get => _agentTypeId; set => _agentTypeId = value; }
        public NavMeshPath Path { get => _navMeshPath; }
        public Vector3 FaceDirection { get => Quaternion.AngleAxis(_faceForwardDegrees, Vector3.up) * transform.forward; }

        public bool HasTrackingAbility { get => _hasTrackAbility; }
        public float TrackRefreshRate { get => _trackRefreshRate; set { _trackRefreshRate = Mathf.Clamp(value, 0, float.MaxValue); } }
        public Transform TrackedTransform { get => _trackedTransform; }
        public bool IsTracking { get => _trackedTransform != null; }

        public float BasePositionalSpeed { get => _basePositionalSpeed; set => _basePositionalSpeed = Mathf.Clamp(value, 0, _terminalPositionalSpeed); }
        public float TerminalPositionalSpeed { get => _terminalPositionalSpeed; set => _terminalPositionalSpeed = Mathf.Clamp(value, _basePositionalSpeed, float.MaxValue);  }
        public float PositionalAcceleration { get => _positionalAcceleration; set => _positionalAcceleration = Mathf.Clamp(value, 0, float.MaxValue); }
        public float PositionalDeacceleration { get => _positionalDeacceleration; set => _positionalDeacceleration = Mathf.Clamp(value, 0, float.MaxValue); }
        public float StoppingBoundaryDistance { get => _stoppingBoundaryDistance; set => _stoppingBoundaryDistance = Mathf.Clamp(value, 0, float.MaxValue); }
        public bool IsDeacceleratingOnTrack { get => _isSmoothPositionalStoppingEnabled; set => _isSmoothPositionalStoppingEnabled = value; }
        public float TurningAngleTolerance { get => _turningAngleTolerance; set => _turningAngleTolerance = Mathf.Clamp(value, 0, 180); }

        public float BaseAngularSpeed { get => _baseAngularSpeed; set => _baseAngularSpeed = Mathf.Clamp(value, 0, _terminalAngularSpeed); }
        public float TerminalAngularSpeed { get => _terminalAngularSpeed; set => _terminalAngularSpeed = Mathf.Clamp(value, _baseAngularSpeed, float.MaxValue); }
        public float AngularAcceleration { get => _angularAcceleration; set => _angularAcceleration = Mathf.Clamp(value, 0, float.MaxValue); }
        public float StoppingAngle { get => _stoppingAngle; set => _stoppingAngle = Mathf.Clamp(value, 0, 180); }
        public float TurnBoundaryDistance { get => _turnBoundaryDistance; set => _turnBoundaryDistance = value; }

        public LayerMask AlignmentLayers { get => _alignmentLayers; set => _alignmentLayers = value; }
        public float AlignmentRotationalSpeed { get => _alignmentRotationalSpeed; set => _alignmentRotationalSpeed = Mathf.Clamp(value, 0.01f, float.MaxValue); }

        public float PositionalSpeed { get => _positionalSpeed; }
        public float AngularSpeed { get => _angularSpeed; }
        public bool HasPath { get => (_navMeshPath.status == NavMeshPathStatus.PathPartial || _navMeshPath.status == NavMeshPathStatus.PathComplete) && _navMeshPath.corners.Length > 1; }


        public bool IsLayerInNavigableLayers(int layer)
        {
            return _alignmentLayers == (_alignmentLayers | 1 << layer);
        }

        public void ClearPath()
        {
            if (_trackingCoroutine != null)
            {
                StopCoroutine(_trackingCoroutine);
                _trackingCoroutine = null;
                _trackedTransform = null;
            }
            _navMeshPath.ClearCorners();
            _path = new Vector3[0];
        }

        public bool CreatePath(Vector3 targetPosition)
        {
            NavMeshQueryFilter filter = new NavMeshQueryFilter
            {
                areaMask = _areaMask,
                agentTypeID = _agentTypeId
            };

            if (NavMesh.CalculatePath(transform.position, targetPosition, filter, _navMeshPath))
            {
                _path = _navMeshPath.corners;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CreateTrackingPath(Transform targetTransform)
        {
            _trackedTransform = targetTransform;
            _trackingCoroutine = RefreshTrackingPath(targetTransform);

            StartCoroutine(_trackingCoroutine);
        }

        public float GetRemainingPathDistance()
        {
            float totalDistance = 0;
            for (int cornerIndex = 0; cornerIndex < _path.Length - 1; cornerIndex++)
            {
                totalDistance += (_path[cornerIndex] - _path[cornerIndex + 1]).magnitude;
            }
            return totalDistance;
        }

        public void SetPath(NavMeshPath path)
        {
            _navMeshPath = path;
            _path = path.corners;
        }

        public void RotateTowardsPosition(Vector3 position)
        {
            _lookDirection = (position - transform.position).normalized;
        }

        public void Rotate(float angle)
        {
            if (_lookDirection != Vector3.zero)
            {
                _lookDirection = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
            }
            else
            {
                _lookDirection = Quaternion.AngleAxis(angle, Vector3.up) * _lookDirection;
            }
        }

        public void RotateFaceDirection(float angleOffset)
        {
            _faceForwardDegrees += angleOffset % 360;
        }

        public void SetFaceDirection(Vector3 direction)
        {
            _faceForwardDegrees = (Vector3.SignedAngle(FaceDirection, direction, Vector3.up) + _faceForwardDegrees) % 360;
        }

        private IEnumerator RefreshTrackingPath(Transform trackedTransform)
        {
            while (_trackedTransform == trackedTransform)
            {
                CreatePath(_trackedTransform.position);
                yield return new WaitForSeconds(_trackRefreshRate);
            }
        }

        private void ScreenToNavigableWorldPosition(Vector2 screenPosition)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(screenPosition), out RaycastHit hit) && IsLayerInNavigableLayers(hit.transform.gameObject.layer))
            {
                ClearPath();
                if (_hasTrackAbility && hit.rigidbody != null)
                {
                    CreateTrackingPath(hit.transform);
                }
                else
                {
                    NavMeshQueryFilter filter = new NavMeshQueryFilter
                    {
                        areaMask = _areaMask,
                        agentTypeID = _agentTypeId
                    };
                    NavMeshHit navHit;
                    if (NavMesh.SamplePosition(hit.point, out navHit, float.MaxValue, filter))
                    {
                        Debug.DrawLine(navHit.position, new Vector3(navHit.position.x, navHit.position.y + 100, navHit.position.z), Color.magenta, 10);
                        CreatePath(navHit.position);
                    }
                }
            }
        }

        private void PerformPlayerInputAction(InputAction.CallbackContext context)
        {
            Debug.Log("Moving " + gameObject.name);
            ScreenToNavigableWorldPosition(Pointer.current.position.ReadValue());
            _playerEntityPositionController.Disable();
        }

        private void CheckTurningBoundary(float distanceToWaypoint)
        {
            if (distanceToWaypoint <= _turnBoundaryDistance && _path.Length > 2)
            {
                ArrayUtility.RemoveAt(ref _path, 1);
                if (_angularSpeed < _baseAngularSpeed)
                {
                    _angularSpeed = _baseAngularSpeed;
                }
            }
        }

        private void SmoothStoppingPositionalSpeed()
        {
            float distanceTravelRemaining = GetRemainingPathDistance();
            if (distanceTravelRemaining <= _stoppingBoundaryDistance)
            {
                float targetSpeed = (_terminalPositionalSpeed - _basePositionalSpeed) * (distanceTravelRemaining / _stoppingBoundaryDistance) + _basePositionalSpeed;
                if (_positionalSpeed > targetSpeed)
                {
                    _positionalSpeed = targetSpeed;
                }
            }
        }

        private void SmoothStoppingAngularSpeed(float turningAngle)
        {
            if (turningAngle <= _stoppingAngle)
            {
                float targetSpeed = ((_terminalAngularSpeed - _baseAngularSpeed) * (turningAngle / _stoppingAngle)) + _baseAngularSpeed;
                if (_angularSpeed > targetSpeed)
                {
                    _angularSpeed = targetSpeed;
                }
            }
        }

        private void CalculateTurningPositionalSpeed(float turningAngle)
        {
            float calculatedAngleTolerance = _turningAngleTolerance / 2;
            float targetSpeed = _terminalPositionalSpeed * (1 - Mathf.Clamp01(turningAngle / calculatedAngleTolerance));
            if (turningAngle < calculatedAngleTolerance && _positionalSpeed < _basePositionalSpeed)
            {
                _positionalSpeed = _basePositionalSpeed;
            }
            if (targetSpeed < _positionalSpeed)
            {
                _positionalSpeed = Mathf.MoveTowards(_positionalSpeed, targetSpeed, _positionalDeacceleration * Time.fixedDeltaTime);
            }
            else
            {
                _positionalSpeed = Mathf.MoveTowards(_positionalSpeed, targetSpeed, _positionalAcceleration * Time.fixedDeltaTime);
            }
        }

        private void CalculateTurningAngularSpeed(Vector3 targetTurnDirection)
        {
            if (_turnDirection == Vector3.zero)
            {
                _turnDirection = targetTurnDirection;
            }

            if (targetTurnDirection == _turnDirection)
            {
                _angularSpeed = Mathf.MoveTowards(_angularSpeed, _terminalAngularSpeed, _angularAcceleration * Time.fixedDeltaTime);
            }
            else
            {
                _angularSpeed = Mathf.MoveTowards(_angularSpeed, _baseAngularSpeed, _angularDeacceleration * Time.fixedDeltaTime);
                if (_angularSpeed == _baseAngularSpeed)
                {
                    _turnDirection = targetTurnDirection;
                }
            }
        }

        private Vector3 CalculateTurnAngleDirection(float signedTurnAngle)
        {
            if (signedTurnAngle > 0) { return Vector3.down; }
            else if (signedTurnAngle < 0) { return Vector3.up; }
            else { return Vector3.zero; }
        }

        private void UpdatePath()
        {
            Vector3 faceDirection = FaceDirection;
            Vector3 targetDirection = _path[1] - transform.position;
            float distanceToWaypoint = targetDirection.magnitude;
            float turnAngle = Vector3.SignedAngle(new Vector3(targetDirection.x, 0, targetDirection.z), new Vector3(faceDirection.x, 0, faceDirection.z), Vector3.up);
            float absoluteTurnAngle = Mathf.Abs(turnAngle);

            CheckTurningBoundary(distanceToWaypoint);
            if (turnAngle == 0)
            {
                _angularSpeed = 0;
                _turnDirection = Vector3.zero;
                _positionalSpeed = Mathf.MoveTowards(_positionalSpeed, _terminalPositionalSpeed, _positionalAcceleration * Time.fixedDeltaTime);
            }
            else
            {
                Vector3 targetTurnDirection = CalculateTurnAngleDirection(turnAngle);
                CalculateTurningAngularSpeed(targetTurnDirection);
                CalculateTurningPositionalSpeed(absoluteTurnAngle);
                SmoothStoppingAngularSpeed(absoluteTurnAngle);
            }
            SmoothStoppingPositionalSpeed();

            float positionalDelta = Mathf.Clamp(_positionalSpeed * Time.fixedDeltaTime, 0, distanceToWaypoint);
            float angularDelta = Mathf.Clamp(_angularSpeed * Time.fixedDeltaTime, 0, absoluteTurnAngle);

            transform.position += faceDirection * positionalDelta;
            transform.rotation *= Quaternion.AngleAxis(angularDelta, _turnDirection);
            _path[0] = transform.position;

            if (absoluteTurnAngle < 90)
            {
                Vector3 updatedTargetDifference = transform.position - _path[1];
                if (Vector3.Angle(new Vector3(-faceDirection.x, 0, -faceDirection.z), new Vector3(updatedTargetDifference.x, 0, updatedTargetDifference.z)) > 90)
                {
                    if (distanceToWaypoint <= positionalDelta)
                    {
                        transform.position = _path[1];
                    }
                    ArrayUtility.RemoveAt(ref _path, 1);
                    if (_path.Length < 2)
                    {
                        _positionalSpeed = 0;
                        _angularSpeed = 0;
                        ClearPath();
                    }
                }
            }
        }

        private void StopPositionalMovement()
        {
            if (_positionalSpeed > _basePositionalSpeed)
            {
                _positionalSpeed = Mathf.MoveTowards(_positionalSpeed, _basePositionalSpeed, _positionalDeacceleration * Time.fixedDeltaTime);
                Vector3 positionalDisplacement = _positionalSpeed * Time.fixedDeltaTime * transform.forward;
                transform.position += new Vector3(positionalDisplacement.x, 0, positionalDisplacement.z);
            }
            else
            {
                _positionalSpeed = 0;
            }
        }

        private void StopAngularMovement()
        {
            if (_angularSpeed > _baseAngularSpeed)
            {
                _angularSpeed = Mathf.MoveTowards(_angularSpeed, _baseAngularSpeed, _angularDeacceleration * Time.fixedDeltaTime);
                float angleDegree = _angularSpeed * Time.fixedDeltaTime;
                transform.rotation *= Quaternion.AngleAxis(angleDegree, _turnDirection);
            }
            else
            {
                _angularSpeed = 0;
            }
        }

        private void UpdateLookDirection()
        {
            Vector3 faceDirection = FaceDirection;
            float turnAngle = Vector3.SignedAngle(new Vector3(_lookDirection.x, 0, _lookDirection.z), new Vector3(faceDirection.x, 0, faceDirection.z), Vector3.up);
            float absoluteTurnAngle = Mathf.Abs(turnAngle);

            if (absoluteTurnAngle != 0)
            {
                Vector3 targetTurnDirection = CalculateTurnAngleDirection(turnAngle);
                CalculateTurningAngularSpeed(targetTurnDirection);
                SmoothStoppingAngularSpeed(absoluteTurnAngle);
                
                float angleDegree = Mathf.Clamp(_angularSpeed * Time.fixedDeltaTime, 0, absoluteTurnAngle);
                transform.rotation *= Quaternion.AngleAxis(angleDegree, _turnDirection);
            }
            else
            {
                _lookDirection = Vector3.zero;
                _turnDirection = Vector3.zero;
                _angularSpeed = 0;
            }
        }

        private void AlignToNavigableLayer()
        {
            NavMeshQueryFilter filter = new NavMeshQueryFilter
            {
                areaMask = _areaMask,
                agentTypeID = _agentTypeId
            };
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit navHit, float.MaxValue, filter))
            {
                transform.position = new Vector3(transform.position.x, navHit.position.y, transform.position.z);
            }

            Ray ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, _alignmentLayers))
            {
                Quaternion pitchRotation = Quaternion.FromToRotation(new Vector3(0, transform.up.y, transform.up.z), new Vector3(0, hit.normal.y, hit.normal.z));
                Quaternion rollRotation = Quaternion.FromToRotation(new Vector3(transform.up.x, transform.up.y, 0), new Vector3(hit.normal.x, hit.normal.y, 0));
                Quaternion targetRotation = pitchRotation * rollRotation * transform.rotation;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _alignmentRotationalSpeed * Time.fixedDeltaTime);
            }
            else
            {
                Quaternion targetRotation = Quaternion.FromToRotation(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y, 0));
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _alignmentRotationalSpeed * Time.fixedDeltaTime);
                //transform.eulerAngles = Vector3.RotateTowards(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y, 0), _terminalAngularSpeed * Time.fixedDeltaTime, 1);
            }
        }

        private void FixedUpdate()
        {
            if (HasPath)
            {
                UpdatePath();
            }
            else
            {
                StopPositionalMovement();
                if (_lookDirection != Vector3.zero)
                {
                    UpdateLookDirection();
                }
                else
                {
                    StopAngularMovement();
                }
            }
            AlignToNavigableLayer();
        }

        private void Awake()
        {
            _navMeshPath = new NavMeshPath();
        }

        private void OnEnable()
        {
            if (_playerEntityPositionController == null)
            {
                _playerEntityPositionController = new PlayerEntityPositionController();
            }
            _playerEntityPositionController.PlayerEntityPositioner.MoveToPosition.performed += PerformPlayerInputAction;
        }

        private void OnDisable()
        {
            _playerEntityPositionController.PlayerEntityPositioner.MoveToPosition.performed -= PerformPlayerInputAction;
        }

        private void OnValidate()
        {
            if (_terminalPositionalSpeed < _basePositionalSpeed)
            {
                _basePositionalSpeed = _terminalPositionalSpeed;
            }
            if (_terminalAngularSpeed < _baseAngularSpeed)
            {
                _baseAngularSpeed = _terminalAngularSpeed;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            for (int cornerIndex = 0; cornerIndex < _path.Length; cornerIndex++)
            {
                Gizmos.DrawSphere(_path[cornerIndex], 0.5f);
            }
            for (int cornerIndex = 1; cornerIndex < _path.Length; cornerIndex++)
            {
                Gizmos.DrawLine(_path[cornerIndex - 1], _path[cornerIndex]);
            }
        }
    }
}