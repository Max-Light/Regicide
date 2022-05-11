using Regicide.Game.Player;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Regicide.Game.BattleFormation
{
    public class BattleLineController : MonoBehaviour
    {
        public static class ActivatedControlPoint
        {
            public static IControlPoint controlPoint;
            public static Vector3 pointerOffset;
        }

        [SerializeField] private BattleLineSpline _battleLineSpline = null;
        [SerializeField] private EdgeControlPoint _edgeControlPointPrefab = null;
        [SerializeField] private VertexControlPoint _vertexControlPointPrefab = null;
        [SerializeField] private AnchorControlPoint _anchorControlPointPrefab = null;

        public FormationUnit[] tests;

        private Dictionary<SplineNode, VertexControlPoint> _vertexControlPoints = new Dictionary<SplineNode, VertexControlPoint>();
        private Dictionary<FormationNode, EdgeControlPoint> _edgeControlPoints = new Dictionary<FormationNode, EdgeControlPoint>();
        private Dictionary<SplineAnchorPair, AnchorControlPoint> _anchorControlPoints = new Dictionary<SplineAnchorPair, AnchorControlPoint>();

        public BattleLineSpline BattleLine { get => _battleLineSpline; }

        public EdgeControlPoint GenerateEdgePoint(FormationNode node)
        {
            if (!_edgeControlPoints.ContainsKey(node) && !(node.PrevNode != null && node.PrevNode is SplineNode) && !(node.NextNode != null && node.NextNode is SplineNode))
            {
                EdgeControlPoint edgePoint = Instantiate(_edgeControlPointPrefab.gameObject, transform).GetComponent<EdgeControlPoint>();
                edgePoint.FormationNode = node;
                _edgeControlPoints.Add(node, edgePoint);

                if (node is SplineNode && _vertexControlPoints.TryGetValue(node as SplineNode, out VertexControlPoint vertexPoint))
                {
                    _battleLineSpline.RevertSplineNodeAsFormationNode(node as SplineNode);
                    GenerateEdgePoint(node.PrevNode);
                    GenerateEdgePoint(node.NextNode);
                }
                return edgePoint;
            }
            return null;
        }

        public VertexControlPoint GenerateVertexPoint(SplineNode node)
        {
            if (!_vertexControlPoints.ContainsKey(node) && !(node.PrevNode != null && node.PrevNode is SplineNode) && !(node.NextNode != null && node.NextNode is SplineNode))
            {
                VertexControlPoint vertexPoint = Instantiate(_vertexControlPointPrefab.gameObject, transform).GetComponent<VertexControlPoint>();
                vertexPoint.SplineNode = node;
                _vertexControlPoints.Add(node, vertexPoint);

                EdgeControlPoint edgePoint;
                if (node.PrevNode != null && _edgeControlPoints.TryGetValue(node.PrevNode, out edgePoint))
                {
                    Destroy(edgePoint.gameObject);
                    _edgeControlPoints.Remove(node.PrevNode);
                }
                if (node.NextNode != null && _edgeControlPoints.TryGetValue(node.NextNode, out edgePoint))
                {
                    Destroy(edgePoint.gameObject);
                    _edgeControlPoints.Remove(node.NextNode);
                }
                return vertexPoint;
            }
            return null;
        }

        public AnchorControlPoint GenerateAnchorPoint(SplineAnchorPair anchorPair)
        {
            if (!_anchorControlPoints.ContainsKey(anchorPair))
            {
                AnchorControlPoint anchorPoint = Instantiate(_anchorControlPointPrefab.gameObject, transform).GetComponent<AnchorControlPoint>();
                anchorPoint.OutAnchorArm.Anchor = anchorPair.OutAnchor;
                anchorPoint.InAnchorArm.Anchor = anchorPair.InAnchor;
                _anchorControlPoints.Add(anchorPair, anchorPoint);
                return anchorPoint;
            }
            return null;
        }

        public bool SetAsVertexPoint(EdgeControlPoint edgePoint)
        {
            SplineNode splineNode = _battleLineSpline.SetFormationNodeAsSplineNode(edgePoint.FormationNode);
            bool isActivePoint = ActivatedControlPoint.controlPoint == (IControlPoint)edgePoint;
            VertexControlPoint vertexPoint = GenerateVertexPoint(splineNode);
            if (vertexPoint != null)
            {
                _edgeControlPoints.Remove(edgePoint.FormationNode);
                Destroy(edgePoint.gameObject);
                int curveIndex = _battleLineSpline.GetCurveIndexOf(splineNode);
                GenerateAnchorPoint(_battleLineSpline.SplineAnchors[curveIndex]);
                if (isActivePoint)
                {
                    ActivatedControlPoint.controlPoint = vertexPoint;
                }
                return true;
            }
            return false;
        }

        private void InitializeControlPoints()
        {
            IReadOnlyList<SplineNode> splineNodes = _battleLineSpline.SplineNodes;
            IReadOnlyList<SplineAnchorPair> splineAnchors = _battleLineSpline.SplineAnchors;
            int curveCount = _battleLineSpline.CurveCount;

            for (int curveIndex = 0; curveIndex < curveCount; curveIndex++)
            {
                SplineNode startNode = splineNodes[curveIndex];
                SplineNode endNode = splineNodes[curveIndex + 1];
                GenerateVertexPoint(startNode);
                GenerateAnchorPoint(splineAnchors[curveIndex]);
                FormationNode node = startNode.NextNode;
                if (node != endNode && node.NextNode != endNode)
                {
                    node = node.NextNode;
                    while (node.NextNode != endNode)
                    {
                        GenerateEdgePoint(node);
                        node = node.NextNode;
                    }
                }
            }
            GenerateVertexPoint(splineNodes[curveCount]);
        }

        private void SelectControlPoint(InputAction.CallbackContext context)
        {
            Ray pointerRay = GamePlayer.LocalGamePlayer.MainCamera.ScreenPointToRay(Pointer.current.position.ReadValue());
            if (Physics.Raycast(pointerRay, out RaycastHit hit) && hit.collider.TryGetComponent(out IControlPoint controlPoint))
            {
                ActivatedControlPoint.controlPoint = controlPoint;
                ActivatedControlPoint.pointerOffset = controlPoint.transform.position - hit.point;
            }
        }

        private IEnumerator UpdateControlPointPosition()
        {
            while (ActivatedControlPoint.controlPoint != null)
            {
                Ray pointerRay = GamePlayer.LocalGamePlayer.MainCamera.ScreenPointToRay(Pointer.current.position.ReadValue());
                if (Physics.Raycast(pointerRay, out RaycastHit hit))
                {
                    ActivatedControlPoint.controlPoint.OnDragPoint(this, transform.InverseTransformPoint(hit.point));
                }
                yield return null;
            }
        }

        private void DragControlPoint(InputAction.CallbackContext context)
        {
            StartCoroutine(nameof(UpdateControlPointPosition));
        }

        private void DeselectControlPoint(InputAction.CallbackContext context)
        {
            if (ActivatedControlPoint.controlPoint != null)
            {
                ActivatedControlPoint.controlPoint = null;
                ActivatedControlPoint.pointerOffset = Vector3.zero;
            }
        }

        private void InitializeControlPointDrag(PlayerInputController controller)
        {
            controller.BattleFormationControl.DragControlPoint.started += SelectControlPoint;
            controller.BattleFormationControl.DragControlPoint.performed += DragControlPoint;
            controller.BattleFormationControl.DragControlPoint.canceled += DeselectControlPoint;
        }

        private void Start()
        {
            PlayerInputController controller = GamePlayer.LocalGamePlayer.PlayerInputControl;
            InitializeControlPointDrag(controller);
            _battleLineSpline.ResetBattleLine(tests);
            InitializeControlPoints();
            
        }

        private void Awake()
        {
            
        }

        private void OnValidate()
        {
            _battleLineSpline = GetComponent<BattleLineSpline>();
        }
    }
}