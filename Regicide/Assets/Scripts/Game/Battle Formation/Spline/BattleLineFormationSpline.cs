
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Regicide.Game.BattleFormation
{
    public class BattleLineFormationSpline : MonoBehaviour
    {
        [SerializeField] private BattleLineFormationNode[] _battleLineNodes;
        [SerializeField] private List<BattleLineFormationNode> _splineNodes = new List<BattleLineFormationNode>();
        private List<Vector3> _splineNodeDisplacementSnapshot = new List<Vector3>();

        public IReadOnlyList<BattleLineFormationNode> SplineNodes { get => _splineNodes; }
        public BattleLineFormationNode[] BattleLineNodes { get => _battleLineNodes; }
        public int CurveCount { get => _splineNodes.Count - 1; }

        public Vector3 GetPosition(float t)
        {
            t = Mathf.Clamp01(t);
            int curveIndex;
            if (t == 1)
            {
                curveIndex = _splineNodes.Count - 2;
            }
            else
            {
                t *= CurveCount;
                curveIndex = (int)t;
                t -= curveIndex;
            }
            return GetPosition(t, curveIndex);
        }

        public Vector3 GetPosition(float t, int curveIndex)
        {
            t = Mathf.Clamp01(t);
            float difference = 1f - t;

            BattleLineFormationNode point0 = _splineNodes[curveIndex];
            BattleLineFormationNode point1 = _splineNodes[curveIndex + 1];

            return transform.TransformPoint(
                Mathf.Pow(difference, 3) * point0.Position +
                3f * Mathf.Pow(difference, 2) * t * point0.OutAnchor.Position +
                3f * difference * Mathf.Pow(t, 2) * point1.InAnchor.Position +
                Mathf.Pow(t, 3) * point1.Position
                );
        }

        public Vector3 GetTangent(float t)
        {
            t = Mathf.Clamp01(t);
            int curveIndex;
            if (t == 1)
            {
                curveIndex = _splineNodes.Count - 2;
            }
            else
            {
                t *= CurveCount;
                curveIndex = (int)t;
                t -= curveIndex;
            }
            return GetTangent(t, curveIndex);
        }

        public Vector3 GetTangent(float t, int curveIndex)
        {
            t = Mathf.Clamp01(t);
            float difference = 1f - t;

            BattleLineFormationNode point0 = _splineNodes[curveIndex];
            BattleLineFormationNode point1 = _splineNodes[curveIndex + 1];

            return transform.TransformPoint(
                3f * Mathf.Pow(difference, 2) * (point0.OutAnchor.Position - point0.Position) +
                6f * difference * t * (point1.InAnchor.Position - point0.OutAnchor.Position) +
                3f * Mathf.Pow(t, 2) * (point1.Position - point1.InAnchor.Position)
                ) - transform.position;
        }

        public void ResizeBattleLine(int size)
        {
            if (size > _battleLineNodes.Length)
            {
                int originalLength = _battleLineNodes.Length;
                Array.Resize(ref _battleLineNodes, size);
                BattleLineFormationNode endNode;

                for (int nodeIndex = originalLength; nodeIndex < size; nodeIndex++)
                {
                    endNode = _battleLineNodes[nodeIndex - 1];
                    endNode.OutAnchor = new BattleLineFormationAnchor();
                    endNode.OutAnchor.Position = new Vector3(endNode.Position.x + 1, 0, 0);

                    BattleLineFormationNode node = new BattleLineFormationNode();
                    node.InAnchor = new BattleLineFormationAnchor();
                    node.InAnchor.Position = new Vector3(endNode.Position.x + 2, 0, 0);
                    node.Position = new Vector3(endNode.Position.x + 3, 0, 0);
                    _battleLineNodes[nodeIndex] = node;
                }

                endNode = _battleLineNodes[size - 1];
                _splineNodes[CurveCount] = endNode;
                endNode.OutAnchor = null;
            }
            else if (size > 2)
            {
                
            }
        }

        public void AppendFormationNode(BattleLineFormationNode node)
        {
            Debug.Log("Adding node");
            Array.Resize(ref _battleLineNodes, _battleLineNodes.Length + 1);
            _battleLineNodes[_battleLineNodes.Length - 1] = node;
            BattleLineFormationNode innerNode = _battleLineNodes[_battleLineNodes.Length - 2];

            Vector3 normalizedDirection = (_splineNodes[_splineNodes.Count - 1].Position - _splineNodes[_splineNodes.Count - 2].Position).normalized;
            Vector3 normalizedAnchor = (innerNode.Position - innerNode.InAnchor.Position).normalized;
            float lengthBetweenNodes = node.Radius + innerNode.Radius;
            Vector3 anchorOffset = (1 / 3) * lengthBetweenNodes * normalizedAnchor;

            innerNode.OutAnchor = new BattleLineFormationAnchor();
            bool isAnchorActive = innerNode.InAnchor.Active;
            innerNode.InAnchor.Active = false;

            node.Position = lengthBetweenNodes * normalizedDirection + innerNode.Position;
            node.InAnchor = new BattleLineFormationAnchor();
            node.InAnchor.Position = node.Position + anchorOffset;
            node.InAnchor.Active = isAnchorActive;

            _splineNodes[CurveCount] = node;
        }

        public bool AddNodeToSpline(BattleLineFormationNode formationNode)
        {
            int splineIndex = 0;
            for (int nodeIndex = 1; nodeIndex < _battleLineNodes.Length - 1; nodeIndex++)
            {
                if (_battleLineNodes[nodeIndex] == _splineNodes[splineIndex + 1])
                {
                    splineIndex++;
                }
                else if (_battleLineNodes[nodeIndex] == formationNode)
                {
                    if (_battleLineNodes[nodeIndex - 1] == _splineNodes[splineIndex] || _battleLineNodes[nodeIndex + 1] == _splineNodes[splineIndex + 1])
                    {
                        return false;
                    }
                    _splineNodes.Insert(splineIndex + 1, formationNode);
                    _splineNodeDisplacementSnapshot.Insert(splineIndex + 1, _splineNodes[splineIndex + 2].Position - formationNode.Position);
                    _splineNodeDisplacementSnapshot[splineIndex] = formationNode.Position - _splineNodes[splineIndex].Position;
                    return true;
                }
            }
            return false;
        }

        public bool RemoveNodeFromSpline(BattleLineFormationNode formationNode)
        {
            int splineIndex = _splineNodes.IndexOf(formationNode);
            if (splineIndex > 0 && splineIndex < _splineNodes.Count - 1)
            {
                _splineNodes.RemoveAt(splineIndex);
                _splineNodeDisplacementSnapshot[splineIndex - 1] = _splineNodes[splineIndex].Position - _splineNodes[splineIndex - 1].Position;
                _splineNodeDisplacementSnapshot.RemoveAt(splineIndex + 1);
                return true;
            }
            return false;
        }

        public bool RemoveNodeFromSplineAt(int index)
        {
            return false;
        }

        public void CalculateSpline()
        {
            int nodeIndex = 0;
            int curveCount = CurveCount;
            for (int curveIndex = 0; curveIndex < curveCount; curveIndex++)
            {
                int endCurveIndex = curveIndex + 1;
                BattleLineFormationNode startNode = _splineNodes[curveIndex];
                BattleLineFormationNode endNode = _splineNodes[endCurveIndex];
                BattleLineFormationNode node = _battleLineNodes[++nodeIndex];
                
                Vector3 anchorInOffset;
                Vector3 anchorOutOffset;
                Vector3 curveDisplacement = endNode.Position - startNode.Position;
                Vector3 curveDirection = curveDisplacement.normalized;

                int nodeRadiusCounterIndex = nodeIndex;
                int nodeCount = 1;
                float splineLength = startNode.Radius + endNode.Radius;
                while (node != endNode)
                {
                    nodeCount++;
                    nodeRadiusCounterIndex++;
                    splineLength += 2 * node.Radius;
                    node = _battleLineNodes[nodeRadiusCounterIndex];
                }

                if (!startNode.OutAnchor.Active && !endNode.InAnchor.Active)
                {
                    anchorOutOffset = (1f / 3f) * splineLength * curveDirection;
                    anchorInOffset = -anchorOutOffset;
                }
                else if (startNode.OutAnchor.Active && !endNode.InAnchor.Active)
                {
                    Vector3 normalizedAnchor = (startNode.OutAnchor.Position - startNode.Position).normalized;
                    anchorOutOffset = (1f / 3f) * splineLength * normalizedAnchor;
                    anchorInOffset = Vector3.Reflect(anchorOutOffset, curveDirection);
                }
                else if (!startNode.OutAnchor.Active && endNode.InAnchor.Active)
                {
                    Vector3 normalizedAnchor = (endNode.InAnchor.Position - endNode.Position).normalized;
                    anchorInOffset = (1f / 3f) * splineLength * normalizedAnchor;
                    anchorOutOffset = Vector3.Reflect(anchorInOffset, curveDirection);
                }
                else
                {
                    Vector3 normalizedOutAnchor = (startNode.OutAnchor.Position - startNode.Position).normalized;
                    Vector3 normalizedInAnchor = (endNode.InAnchor.Position - endNode.Position).normalized;
                    float anchorMagnitude = (1f / 3f) * splineLength;
                    anchorOutOffset = anchorMagnitude * normalizedOutAnchor;
                    anchorInOffset = anchorMagnitude * normalizedInAnchor;
                }

                Vector3 updatedCurveDisplacement = curveDirection * splineLength;
                endNode.Position = startNode.Position + updatedCurveDisplacement;
                
                if (endCurveIndex != curveCount && _splineNodeDisplacementSnapshot[curveIndex] != updatedCurveDisplacement)
                {
                    int endCurveSegmentIndex = endCurveIndex + 1;
                    Vector3 splineSegmentDisplacement = _splineNodes[endCurveSegmentIndex].Position - startNode.Position;
                    Vector3 nodeDisplacementDifference = _splineNodeDisplacementSnapshot[curveIndex] - updatedCurveDisplacement;
                    Vector3 reflection = Vector3.Reflect(nodeDisplacementDifference, splineSegmentDisplacement.normalized);
                    Vector3 updatedEndCurveDisplacement = _splineNodeDisplacementSnapshot[endCurveIndex] + reflection;
                    _splineNodes[endCurveSegmentIndex].Position = endNode.Position + updatedEndCurveDisplacement;
                    _splineNodeDisplacementSnapshot[endCurveIndex] = updatedEndCurveDisplacement;

                    for (int splineNodeDisplaceIndex = endCurveSegmentIndex; splineNodeDisplaceIndex < _splineNodeDisplacementSnapshot.Count; splineNodeDisplaceIndex++)
                    {
                        _splineNodes[splineNodeDisplaceIndex + 1].Position = _splineNodes[splineNodeDisplaceIndex].Position + _splineNodeDisplacementSnapshot[splineNodeDisplaceIndex];
                    }
                }
                _splineNodeDisplacementSnapshot[curveIndex] = updatedCurveDisplacement;
                
                startNode.OutAnchor.Position = startNode.Position + anchorOutOffset;
                endNode.InAnchor.Position = endNode.Position + anchorInOffset;

                for (int nodeValue = 1; nodeValue < nodeCount; nodeValue++)
                {
                    float t = (float)nodeValue / nodeCount;
                    _battleLineNodes[nodeIndex].Position = transform.InverseTransformPoint(GetPosition(t, curveIndex));
                    nodeIndex++;
                }
            }
        }

        private void InitializeSpline()
        {
            BattleLineFormationNode startNode = new BattleLineFormationNode();
            BattleLineFormationAnchor anchorOut = new BattleLineFormationAnchor();
            BattleLineFormationAnchor anchorIn = new BattleLineFormationAnchor();
            BattleLineFormationNode endNode = new BattleLineFormationNode();

            startNode.OutAnchor = anchorOut;
            endNode.InAnchor = anchorIn;
            //startNode.OutAnchor.Active = true;

            startNode.Position = new Vector3(1, 0, 0);
            anchorOut.Position = new Vector3(2, 0, 0);
            anchorIn.Position = new Vector3(3, 0, 0);
            endNode.Position = new Vector3(4, 0, 0);

            _battleLineNodes = new BattleLineFormationNode[2] { startNode, endNode };
            _splineNodes = new List<BattleLineFormationNode>() { startNode, endNode };
            _splineNodeDisplacementSnapshot = new List<Vector3>() { endNode.Position - startNode.Position };
            //_splineNodeDisplacementSnapshot = new List<Vector3>() { startNode.Position, endNode.Position - startNode.Position };
        }

        private void Awake()
        {
            InitializeSpline();
        }

        private void Reset()
        {
            InitializeSpline();
        }

        private void Update()
        {
            CalculateSpline();
            if (Keyboard.current.mKey.wasPressedThisFrame)
            {
                AppendFormationNode(new BattleLineFormationNode());
                AppendFormationNode(new BattleLineFormationNode());
                AppendFormationNode(new BattleLineFormationNode());
            }
            if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                AddNodeToSpline(_battleLineNodes[_battleLineNodes.Length / 2]);
            }
        }
    }
}