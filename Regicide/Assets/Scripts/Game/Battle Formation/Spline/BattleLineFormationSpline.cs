
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Regicide.Game.BattleFormation
{
    public class BattleLineFormationSpline : MonoBehaviour
    {
        public enum AnchorMode
        {
            LINEAR,
            QUADRATIC,
            CUBIC,
        }

        public struct BattleLineCurveInfo
        {
            public int nodeCount;
            public float curveLength;
            public int battleLineIndex;

            public int EndNodeBattleLineIndex { get => battleLineIndex + nodeCount - 1; }
        }

        [SerializeField] private BattleLineNode[] _battleLineNodes;
        [SerializeField] private List<BattleLineSplineNode> _splineNodes = new List<BattleLineSplineNode>();
        [SerializeField] private float _maxCurveSegmentAngle = 60f;
        [SerializeField] private float _maxAnchorAngle = 45f;
        private List<AnchorMode> _splineAnchorModes = new List<AnchorMode>();
        private const float _anchorPartition = 1f / 3f;

        public IReadOnlyList<BattleLineSplineNode> SplineNodes { get => _splineNodes; }
        public IReadOnlyList<AnchorMode> SplineAnchorModes { get => _splineAnchorModes; }
        public BattleLineNode[] BattleLineNodes { get => _battleLineNodes; }
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

            BattleLineSplineNode point0 = _splineNodes[curveIndex];
            BattleLineSplineNode point1 = _splineNodes[curveIndex + 1];

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

            BattleLineSplineNode point0 = _splineNodes[curveIndex];
            BattleLineSplineNode point1 = _splineNodes[curveIndex + 1];

            return transform.TransformPoint(
                3f * Mathf.Pow(difference, 2) * (point0.OutAnchor.Position - point0.Position) +
                6f * difference * t * (point1.InAnchor.Position - point0.OutAnchor.Position) +
                3f * Mathf.Pow(t, 2) * (point1.Position - point1.InAnchor.Position)
                ) - transform.position;
        }

        public void AppendFormationNode(BattleLineNode node)
        {
            int curveCount = CurveCount;
            Array.Resize(ref _battleLineNodes, _battleLineNodes.Length + 1);
            BattleLineSplineNode endNode = _splineNodes[curveCount];

            Vector3 normalizedDirection = (_splineNodes[curveCount].Position - _splineNodes[curveCount - 1].Position).normalized;
            float lengthBetweenNodes = node.Radius + endNode.Radius;
            Vector3 displacementOffset = normalizedDirection * lengthBetweenNodes;

            node.Position = endNode.Position;
            endNode.InAnchor.OffsetPosition(displacementOffset);
            endNode.OffsetPosition(displacementOffset);

            _battleLineNodes[_battleLineNodes.Length - 2] = node;
            _battleLineNodes[_battleLineNodes.Length - 1] = endNode;

            BattleLineCurveInfo curveInfo = GetCurveInfoAt(_splineNodes[curveCount - 1], _battleLineNodes.Length - 1);
            RestrictAnchors(curveCount - 1, curveInfo);
        }

        public bool AddNodeToSpline(BattleLineNode formationNode)
        {
            int curveIndex = 0;
            int splineBattleLineIndex = 0;
            for (int nodeIndex = 1; nodeIndex < _battleLineNodes.Length - 1; nodeIndex++)
            {
                if (_battleLineNodes[nodeIndex] == _splineNodes[curveIndex + 1])
                {
                    curveIndex++;
                    splineBattleLineIndex = nodeIndex;
                }
                else if (_battleLineNodes[nodeIndex] == formationNode)
                {
                    int midNodeIndex = curveIndex + 1;
                    if (_battleLineNodes[nodeIndex - 1] == _splineNodes[curveIndex] || _battleLineNodes[nodeIndex + 1] == _splineNodes[midNodeIndex])
                    {
                        return false;
                    }
                    BattleLineSplineNode startNode = _splineNodes[curveIndex];
                    BattleLineSplineNode formationSplineNode = new BattleLineSplineNode(formationNode);
                    _battleLineNodes[nodeIndex] = formationSplineNode;
                    _splineNodes.Insert(midNodeIndex, formationSplineNode);
                    BattleLineSplineNode endNode = _splineNodes[midNodeIndex + 1];

                    _splineNodes[midNodeIndex].SnapshotPosition();
                    _splineAnchorModes.Insert(midNodeIndex, _splineAnchorModes[midNodeIndex - 1]);

                    BattleLineCurveInfo curveInfo;
                    BattleLineSplineAnchor anchor;

                    curveInfo = GetCurveInfoAt(splineBattleLineIndex, formationSplineNode);
                    anchor = new BattleLineSplineAnchor();
                    formationSplineNode.InAnchor = anchor;
                    anchor.Position = (startNode.Position - formationSplineNode.Position).normalized * (_anchorPartition * curveInfo.curveLength) + formationSplineNode.Position;
                    anchor.SnapshotPosition();

                    RestrictAnchors(curveIndex, curveInfo);
                    CompressBattleLineCurve(curveIndex, curveInfo);

                    curveInfo = GetCurveInfoAt(nodeIndex, endNode);
                    anchor = new BattleLineSplineAnchor();
                    formationSplineNode.OutAnchor = anchor;
                    anchor.Position = (endNode.Position - formationSplineNode.Position).normalized * (_anchorPartition * curveInfo.curveLength) + formationSplineNode.Position;
                    anchor.SnapshotPosition();

                    RestrictAnchors(midNodeIndex, curveInfo);
                    CompressBattleLineCurve(midNodeIndex, curveInfo);
                    return true;
                }
            }
            return false;
        }

        public bool AddNodeToSplineAt(int battleLineIndex)
        {
            return AddNodeToSpline(_battleLineNodes[battleLineIndex]);
        }

        public bool RemoveNodeFromSpline(BattleLineSplineNode formationNode)
        {
            int curveIndex = _splineNodes.IndexOf(formationNode);
            if (curveIndex > 0 && curveIndex < CurveCount)
            {
                _splineNodes.RemoveAt(curveIndex);
                _splineAnchorModes.RemoveAt(curveIndex);
                return true;
            }
            return false;
        }

        public bool RemoveNodeFromSplineAt(int index)
        {
            return false;
        }

        public void CalculateBattleLineSpline()
        {
            int nodeIndex = 0;
            int curveIndex;
            int curveCount = CurveCount;

            BattleLineSplineNode startNode = _splineNodes[nodeIndex];
            BattleLineSplineNode endNode = _splineNodes[nodeIndex + 1];
            BattleLineCurveInfo curveInfo;

            startNode.Position = (endNode.Position - startNode.PreviousPosition).magnitude * (startNode.Position - endNode.Position).normalized + endNode.Position;

            for (curveIndex = 0; curveIndex < curveCount - 1; curveIndex++)
            {
                startNode = _splineNodes[curveIndex];
                endNode = _splineNodes[curveIndex + 1];
                curveInfo = GetCurveInfoAt(nodeIndex, endNode);

                MaintainAnchorRotations(curveIndex);
                RestrictAnchors(curveIndex, curveInfo);
                CompressBattleLineCurve(curveIndex, curveInfo);

                if (ReflectCurves(curveIndex))
                {
                    BattleLineSplineNode midNode = endNode;
                    curveIndex++;
                    endNode = _splineNodes[curveIndex + 1];
                    nodeIndex += curveInfo.nodeCount - 1;
                    curveInfo = GetCurveInfoAt(curveInfo.EndNodeBattleLineIndex, endNode);

                    MaintainAnchorRotations(curveIndex);
                    RestrictAnchors(curveIndex, curveInfo);
                    CompressBattleLineCurve(curveIndex, curveInfo);

                    Vector3 curveOffset = endNode.Position - endNode.PreviousPosition;
                    OffsetSplineNodePointsAt(curveIndex + 2, curveOffset);

                    startNode.OutAnchor.SnapshotPosition();
                    midNode.InAnchor.SnapshotPosition();
                    midNode.SnapshotPosition();
                    midNode.OutAnchor.SnapshotPosition();
                    endNode.InAnchor.SnapshotPosition();
                    endNode.SnapshotPosition();

                    nodeIndex += curveInfo.nodeCount - 1;
                }
                else
                {
                    startNode.OutAnchor.SnapshotPosition();
                    endNode.InAnchor.SnapshotPosition();
                    endNode.SnapshotPosition();

                    nodeIndex += curveInfo.nodeCount - 1;
                }
            }

            if (curveIndex == curveCount - 1)
            {
                startNode = _splineNodes[curveIndex];
                endNode = _splineNodes[curveIndex + 1];
                curveInfo = GetCurveInfoAt(nodeIndex, endNode);

                MaintainAnchorRotations(curveIndex);
                RestrictAnchors(curveIndex, curveInfo);
                CompressBattleLineCurve(curveIndex, curveInfo);

                startNode.OutAnchor.SnapshotPosition();
                endNode.InAnchor.SnapshotPosition();
                endNode.SnapshotPosition();
            }

            startNode = _splineNodes[0];
            if (!startNode.IsSamePosition)
            {
                Vector3 startNodeOffset = startNode.PreviousPosition - startNode.Position;
                _splineNodes[0].SnapshotPosition();

                startNode.OffsetPosition(startNodeOffset);
                OffsetSplineNodePointsAt(1, startNodeOffset);
                transform.position -= startNodeOffset;

                curveIndex = 0;
                for (nodeIndex = 0; nodeIndex < _battleLineNodes.Length; nodeIndex++)
                {
                    if (_splineNodes[curveIndex] == _battleLineNodes[nodeIndex])
                    {
                        curveIndex++;
                    }
                    else
                    {
                        _battleLineNodes[nodeIndex].Position += startNodeOffset;
                    }
                }
            }
        }

        private BattleLineCurveInfo GetCurveInfoAt(int battleLineIndex, BattleLineSplineNode endNode)
        {
            BattleLineNode node = _battleLineNodes[battleLineIndex];
            int nodeCount = 0;
            float curveLength = 0;
            if (node != endNode)
            {
                ++nodeCount;
                curveLength += node.Radius;
                while (_battleLineNodes[battleLineIndex + nodeCount] != endNode)
                {
                    node = _battleLineNodes[battleLineIndex + nodeCount];
                    curveLength += 2 * node.Radius;
                    ++nodeCount;
                }
                ++nodeCount;
                curveLength += node.Radius;
            }
            return new BattleLineCurveInfo
            {
                nodeCount = nodeCount,
                curveLength = curveLength,
                battleLineIndex = battleLineIndex
            };
        }

        private BattleLineCurveInfo GetCurveInfoAt(BattleLineSplineNode startNode, int battleLineIndex)
        {
            BattleLineNode node = _battleLineNodes[battleLineIndex];
            int nodeCount = 0;
            float curveLength = 0;
            if (node != startNode)
            {
                ++nodeCount;
                curveLength += node.Radius;
                while (_battleLineNodes[battleLineIndex - nodeCount] != startNode)
                {
                    node = _battleLineNodes[battleLineIndex - nodeCount];
                    curveLength += 2 * node.Radius;
                    ++nodeCount;
                }
                ++nodeCount;
                curveLength += node.Radius;
            }
            return new BattleLineCurveInfo
            {
                nodeCount = nodeCount,
                curveLength = curveLength,
                battleLineIndex = battleLineIndex - nodeCount
            };
        }

        private void MaintainAnchorRotations(int curveIndex)
        {
            BattleLineSplineNode startNode = _splineNodes[curveIndex];
            BattleLineSplineNode endNode = _splineNodes[curveIndex + 1];

            if (!endNode.IsSamePosition || !startNode.IsSamePosition)
            {
                Vector3 curveDisplacementSnapshot = endNode.PreviousPosition - startNode.PreviousPosition;
                Vector3 curveDisplacement = endNode.Position - startNode.Position;
                Vector3 normalizedCurveDisplacement = curveDisplacement.normalized;

                Vector3 outAnchorDisplacementSnapshot = startNode.OutAnchor.PreviousPosition - startNode.PreviousPosition;
                Vector3 inAnchorDisplacementSnapshot = endNode.InAnchor.PreviousPosition - endNode.PreviousPosition;

                Quaternion outAnchorRotation = Quaternion.FromToRotation(curveDisplacementSnapshot, outAnchorDisplacementSnapshot);
                Quaternion inAnchorRotation = Quaternion.FromToRotation(-curveDisplacementSnapshot, inAnchorDisplacementSnapshot);

                startNode.OutAnchor.Position = outAnchorRotation * (normalizedCurveDisplacement * outAnchorDisplacementSnapshot.magnitude) + startNode.Position;
                endNode.InAnchor.Position = inAnchorRotation * -(normalizedCurveDisplacement * inAnchorDisplacementSnapshot.magnitude) + endNode.Position;
            }
        }

        private void RestrictAnchors(int curveIndex, BattleLineCurveInfo curveInfo)
        {
            BattleLineSplineNode startNode = _splineNodes[curveIndex];
            BattleLineSplineNode endNode = _splineNodes[curveIndex + 1];
            float anchorDistance = _anchorPartition * curveInfo.curveLength;

            Vector3 outAnchorDisplacement;
            Vector3 inAnchorDisplacement;

            switch (_splineAnchorModes[curveIndex])
            {
                case AnchorMode.LINEAR:
                    {
                        Vector3 curveDisplacement = endNode.Position - startNode.Position;

                        outAnchorDisplacement = curveDisplacement;
                        inAnchorDisplacement = -curveDisplacement;
                        break;
                    }
                case AnchorMode.QUADRATIC:
                    {
                        Vector3 curveDisplacement = endNode.Position - startNode.Position;
                        Vector3 curveDisplacementSnapshot = endNode.PreviousPosition - startNode.PreviousPosition;

                        outAnchorDisplacement = startNode.OutAnchor.Position - startNode.Position;
                        inAnchorDisplacement = endNode.InAnchor.Position - endNode.Position;

                        Vector3 outAnchorDisplacementSnapshot = startNode.OutAnchor.PreviousPosition - startNode.PreviousPosition;
                        Vector3 inAnchorDisplacementSnapshot = endNode.InAnchor.PreviousPosition - endNode.PreviousPosition;

                        Quaternion outAnchorRotation = Quaternion.FromToRotation(curveDisplacement, outAnchorDisplacement);
                        Quaternion inAnchorRotation = Quaternion.FromToRotation(-curveDisplacement, inAnchorDisplacement);

                        Quaternion outAnchorRotationSnapshot = Quaternion.FromToRotation(curveDisplacementSnapshot, outAnchorDisplacementSnapshot);
                        Quaternion inAnchorRotationSnapshot = Quaternion.FromToRotation(-curveDisplacementSnapshot, inAnchorDisplacementSnapshot);

                        if (outAnchorRotation != outAnchorRotationSnapshot)
                        {
                            inAnchorDisplacement = Vector3.Reflect(outAnchorDisplacement, curveDisplacement.normalized);
                        }
                        else if (inAnchorRotation != inAnchorRotationSnapshot)
                        {
                            outAnchorDisplacement = Vector3.Reflect(inAnchorDisplacement, curveDisplacement.normalized);
                        }
                        break;
                    }
                default:
                case AnchorMode.CUBIC:
                    {
                        Vector3 curveDisplacement = endNode.Position - startNode.Position;
                        Vector3 curveDisplacementSnapshot = endNode.PreviousPosition - startNode.PreviousPosition;

                        outAnchorDisplacement = startNode.OutAnchor.Position - startNode.Position;
                        inAnchorDisplacement = endNode.InAnchor.Position - endNode.Position;

                        Vector3 outAnchorDisplacementSnapshot = startNode.OutAnchor.PreviousPosition - startNode.PreviousPosition;
                        Vector3 inAnchorDisplacementSnapshot = endNode.InAnchor.PreviousPosition - endNode.PreviousPosition;

                        Quaternion outAnchorRotation = Quaternion.FromToRotation(curveDisplacement, outAnchorDisplacement);
                        Quaternion inAnchorRotation = Quaternion.FromToRotation(-curveDisplacement, inAnchorDisplacement);

                        break;
                    }
            }



            outAnchorDisplacement = outAnchorDisplacement.normalized * anchorDistance;
            inAnchorDisplacement = inAnchorDisplacement.normalized * anchorDistance;

            startNode.OutAnchor.Position = startNode.Position + outAnchorDisplacement;
            endNode.InAnchor.Position = endNode.Position + inAnchorDisplacement;
        }

        private void CompressBattleLineCurve(int curveIndex, BattleLineCurveInfo curveInfo)
        {
            BattleLineSplineNode startNode = _splineNodes[curveIndex];
            BattleLineSplineNode endNode = _splineNodes[curveIndex + 1];

            Vector3 inAnchorOffset = endNode.InAnchor.Position - endNode.Position;
            Vector3 normalizedCurveDisplacement = (endNode.Position - startNode.Position).normalized;
            endNode.Position = normalizedCurveDisplacement * curveInfo.curveLength + startNode.Position;
            endNode.InAnchor.Position = endNode.Position + inAnchorOffset;

            int divisibleNodeCount = curveInfo.nodeCount - 1;
            int nodeIndex;
            Vector3 previousCalculatedPosition;
            Vector3 calculatedPosition;

            previousCalculatedPosition = startNode.Position;
            float curveLength = 0;
            for (nodeIndex = 1; nodeIndex < curveInfo.nodeCount; nodeIndex++)
            {
                calculatedPosition = transform.InverseTransformPoint(GetPosition((float)nodeIndex / divisibleNodeCount, curveIndex));
                curveLength += (calculatedPosition - previousCalculatedPosition).magnitude;
                previousCalculatedPosition = calculatedPosition;
            }

            float curveRatio = curveInfo.curveLength / curveLength;
            endNode.Position = normalizedCurveDisplacement * curveRatio * curveInfo.curveLength + startNode.Position;
            endNode.InAnchor.Position = endNode.Position + inAnchorOffset;

            for (nodeIndex = 1; nodeIndex < divisibleNodeCount; nodeIndex++)
            {
                _battleLineNodes[curveInfo.battleLineIndex + nodeIndex].Position = transform.InverseTransformPoint(GetPosition((float)nodeIndex / divisibleNodeCount, curveIndex));
            }
        }

        private bool ReflectCurves(int startNodeIndex)
        {
            int midNodeIndex = startNodeIndex + 1;
            int endNodeIndex = startNodeIndex + 2;

            BattleLineSplineNode startNode = _splineNodes[startNodeIndex];
            BattleLineSplineNode midNode = _splineNodes[midNodeIndex];
            BattleLineSplineNode endNode = _splineNodes[endNodeIndex];

            if (!midNode.IsSamePosition)
            {
                Vector3 midNodePositionDifference = midNode.PreviousPosition - midNode.Position;
                Vector3 splineSegmentDisplacement = endNode.PreviousPosition - startNode.Position;
                Vector3 reflection = Vector3.Reflect(midNodePositionDifference, splineSegmentDisplacement.normalized);
                Vector3 updatedEndNodeDisplacement = endNode.PreviousPosition - midNode.PreviousPosition + reflection;
                Vector3 updatedEndNodePosition = midNode.Position + updatedEndNodeDisplacement;

                endNode.Position = updatedEndNodePosition;
                return true;
            }
            return false;
        }

        private void OffsetSplineNodePointsAt(int index, Vector3 offset)
        {
            int curveCount = CurveCount;
            if (index <= curveCount && index > 0)
            {
                _splineNodes[index - 1].OutAnchor.OffsetPosition(offset);
                for (int nodeIndex = index; nodeIndex < curveCount; nodeIndex++)
                {
                    _splineNodes[nodeIndex].OutAnchor.OffsetPosition(offset);
                    _splineNodes[nodeIndex].OffsetPosition(offset);
                    _splineNodes[nodeIndex].InAnchor.OffsetPosition(offset);
                }
                _splineNodes[curveCount].InAnchor.OffsetPosition(offset);
                _splineNodes[curveCount].OffsetPosition(offset);
            }
        }

        private void OffsetBattleLine(Vector3 offset)
        {
            int nodeIndex;
            int curveIndex;

            for (nodeIndex = 0; nodeIndex < _battleLineNodes.Length; nodeIndex++)
            {

            }
        }

        private void InitializeSpline()
        {
            BattleLineSplineNode startNode = new BattleLineSplineNode();
            BattleLineSplineAnchor outAnchor = new BattleLineSplineAnchor();
            BattleLineSplineAnchor inAnchor = new BattleLineSplineAnchor();
            BattleLineSplineNode endNode = new BattleLineSplineNode();

            startNode.OutAnchor = outAnchor;
            endNode.InAnchor = inAnchor;

            startNode.Position = new Vector3(1, 0, 0);
            outAnchor.Position = new Vector3(2, 0, 0);
            inAnchor.Position = new Vector3(3, 0, 0);
            endNode.Position = new Vector3(4, 0, 0);

            startNode.SnapshotPosition();
            outAnchor.SnapshotPosition();
            inAnchor.SnapshotPosition();
            endNode.SnapshotPosition();

            _battleLineNodes = new BattleLineSplineNode[2] { startNode, endNode };
            _splineNodes = new List<BattleLineSplineNode>() { startNode, endNode };
            _splineAnchorModes = new List<AnchorMode> { AnchorMode.CUBIC };
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
            CalculateBattleLineSpline();
            if (Keyboard.current.mKey.wasPressedThisFrame)
            {
                AppendFormationNode(new BattleLineSplineNode());
                AppendFormationNode(new BattleLineSplineNode());
                AppendFormationNode(new BattleLineSplineNode());
            }
            if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                AddNodeToSpline(_battleLineNodes[_battleLineNodes.Length / 2]);
            }
        }

        private void OnDrawGizmosSelected()
        {
            for (int nodeIndex = 0; nodeIndex < _battleLineNodes.Length; nodeIndex++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.TransformPoint(_battleLineNodes[nodeIndex].Position), _battleLineNodes[nodeIndex].Radius);
            }
        }
    }
}