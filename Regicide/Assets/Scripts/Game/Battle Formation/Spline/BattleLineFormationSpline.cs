
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Path;
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

        [SerializeField] private BattleLineFormationNode[] _battleLineNodes;
        [SerializeField] private List<BattleLineFormationNode> _splineNodes = new List<BattleLineFormationNode>();
        private List<AnchorMode> _splineAnchorModes = new List<AnchorMode>();
        private Vector3 _transformUpdateOffset = Vector3.zero;
        private const float _anchorPartition = 1f / 3f;

        public IReadOnlyList<BattleLineFormationNode> SplineNodes { get => _splineNodes; }
        public IReadOnlyList<AnchorMode> SplineAnchorModes { get => _splineAnchorModes; }
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

        public void AppendFormationNode(BattleLineFormationNode node)
        {
            Array.Resize(ref _battleLineNodes, _battleLineNodes.Length + 1);
            _battleLineNodes[_battleLineNodes.Length - 1] = node;
            BattleLineFormationNode innerNode = _battleLineNodes[_battleLineNodes.Length - 2];

            int curveCount = CurveCount;
            Vector3 normalizedDirection = (_splineNodes[curveCount].Position - _splineNodes[curveCount - 1].Position).normalized;
            Vector3 normalizedAnchor = (innerNode.Position - innerNode.InAnchor.Position).normalized;
            float lengthBetweenNodes = node.Radius + innerNode.Radius;
            Vector3 anchorOffset = _anchorPartition * lengthBetweenNodes * normalizedAnchor;
            Vector3 displacement = lengthBetweenNodes * normalizedDirection;

            innerNode.OutAnchor = new BattleLineFormationAnchor();
            innerNode.OutAnchor.Position = innerNode.Position + anchorOffset;
            node.Position = innerNode.Position + displacement;
            node.InAnchor = new BattleLineFormationAnchor();
            node.InAnchor.Position = node.Position - anchorOffset;

            _splineNodes[curveCount] = node;
            _splineNodes[curveCount].SnapshotPosition();

            //calculate curve info backwards, then restrict anchors and compress battle line
        }

        public bool AddNodeToSpline(BattleLineFormationNode formationNode)
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
                    BattleLineFormationNode startNode = _splineNodes[curveIndex];
                    _splineNodes.Insert(midNodeIndex, formationNode);
                    BattleLineFormationNode endNode = _splineNodes[midNodeIndex + 1];

                    _splineNodes[midNodeIndex].SnapshotPosition();
                    _splineAnchorModes.Insert(midNodeIndex, _splineAnchorModes[midNodeIndex - 1]);

                    BattleLineCurveInfo curveInfo;
                    curveInfo = GetCurveInfoAt(splineBattleLineIndex, formationNode);
                    RestrictAnchorPoints(curveIndex, curveInfo.curveLength);
                    CompressBattleLineCurve(curveIndex, curveInfo);

                    curveInfo = GetCurveInfoAt(nodeIndex, endNode);
                    RestrictAnchorPoints(midNodeIndex, curveInfo.curveLength);
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

        public bool RemoveNodeFromSpline(BattleLineFormationNode formationNode)
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

        public void CalculateSpline()
        {
            int nodeIndex = 0;
            int curveIndex;
            int curveCount = CurveCount;
            bool updatedCurve = false;

            BattleLineFormationNode startNode = _splineNodes[nodeIndex];
            BattleLineFormationNode endNode = _splineNodes[nodeIndex + 1];
            BattleLineCurveInfo curveInfo;
            
            if (!startNode.IsSamePosition())
            {
                Vector3 startNodePosition = (endNode.PositionSnapshot - startNode.PositionSnapshot).magnitude * (startNode.Position - endNode.Position).normalized + endNode.Position;
                Vector3 startNodePositionDifference = startNodePosition - startNode.PositionSnapshot;
                transform.position = transform.TransformPoint(startNodePosition) - startNode.PositionSnapshot;

                for (int splineNodeIndex = 1; splineNodeIndex < _splineNodes.Count; splineNodeIndex++)
                {
                    _splineNodes[splineNodeIndex].Position -= startNodePositionDifference;
                }
                updatedCurve = true;
                startNode.Position = startNode.PositionSnapshot;
            }

            for (curveIndex = 0; curveIndex < curveCount - 1; curveIndex++)
            {
                curveInfo = GetCurveInfoAt(nodeIndex, _splineNodes[curveIndex + 1]);
                RestrictAnchorPoints(curveIndex, curveInfo.curveLength);
                CompressBattleLineCurve(curveIndex, curveInfo);
                if (!updatedCurve)
                {
                    updatedCurve = ReflectCurves(curveIndex);
                }
                nodeIndex += curveInfo.nodeCount - 1;
            }

            curveIndex = curveCount - 1;
            endNode = _splineNodes[curveCount];

            curveInfo = GetCurveInfoAt(nodeIndex, endNode);
            RestrictAnchorPoints(curveIndex, curveInfo.curveLength);
            CompressBattleLineCurve(curveIndex, curveInfo);

            SnapshotSplinePoints();
            transform.position += _transformUpdateOffset;
            _transformUpdateOffset = Vector3.zero;
        }

        private BattleLineCurveInfo GetCurveInfoAt(int battleLineIndex, BattleLineFormationNode endNode)
        {
            BattleLineFormationNode node = _battleLineNodes[battleLineIndex];
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

        private BattleLineCurveInfo GetCurveInfoAt(BattleLineFormationNode startNode, int battleLineIndex)
        {
            BattleLineFormationNode node = _battleLineNodes[battleLineIndex];
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

        private void RestrictAnchorPoints(int curveIndex, float curveLength)
        {
            BattleLineFormationNode startNode = _splineNodes[curveIndex];
            BattleLineFormationNode endNode = _splineNodes[curveIndex + 1];
            Vector3 anchorInOffset;
            Vector3 anchorOutOffset;
            Vector3 curveDirection = (endNode.Position - startNode.Position).normalized;
            float anchorOffsetDistance = _anchorPartition * curveLength;
            switch (_splineAnchorModes[curveIndex])
            {
                case AnchorMode.LINEAR:
                    {
                        startNode.OutAnchor.Rotation = Quaternion.identity;
                        endNode.InAnchor.Rotation = Quaternion.identity;

                        anchorOutOffset = anchorOffsetDistance * curveDirection;
                        anchorInOffset = -anchorOutOffset;
                        break;
                    }
                case AnchorMode.QUADRATIC:
                    {
                        Vector3 leveledSplineSegmentSnapshot = endNode.PositionSnapshot - startNode.PositionSnapshot;
                        Vector3 anchorDisplacementSnapshot = startNode.OutAnchor.Position - startNode.PositionSnapshot;
                        Quaternion anchorArcRotation = Quaternion.FromToRotation(leveledSplineSegmentSnapshot, anchorDisplacementSnapshot);
                        startNode.OutAnchor.Rotation = anchorArcRotation;
                        endNode.InAnchor.Rotation = Quaternion.Inverse(anchorArcRotation);

                        anchorOutOffset = anchorArcRotation * (curveDirection * anchorOffsetDistance);
                        anchorInOffset = Vector3.Reflect(anchorOutOffset, curveDirection);
                        break;
                    }
                default:
                case AnchorMode.CUBIC:
                    {
                        Vector3 leveledSplineSegmentSnapshot = endNode.PositionSnapshot - startNode.PositionSnapshot;

                        Vector3 outAnchorDisplacementSnapshot = startNode.OutAnchor.Position - startNode.PositionSnapshot;
                        Quaternion outAnchorArcRotation = Quaternion.FromToRotation(leveledSplineSegmentSnapshot, outAnchorDisplacementSnapshot);
                        startNode.OutAnchor.Rotation = outAnchorArcRotation;

                        Vector3 inAnchorDisplacementSnapshot = endNode.InAnchor.Position - endNode.PositionSnapshot;
                        Quaternion inAnchorArcRotation = Quaternion.FromToRotation(-leveledSplineSegmentSnapshot, inAnchorDisplacementSnapshot);
                        endNode.InAnchor.Rotation = inAnchorArcRotation;
                        
                        anchorOutOffset = outAnchorArcRotation * (curveDirection * anchorOffsetDistance);
                        anchorInOffset = inAnchorArcRotation * (-curveDirection * anchorOffsetDistance);
                        break;
                    }
            }
            Debug.Log(anchorInOffset);
            startNode.OutAnchor.Position = startNode.Position + anchorOutOffset;
            endNode.InAnchor.Position = endNode.Position + anchorInOffset;
        }

        private void CompressBattleLineCurve(int curveIndex, BattleLineCurveInfo curveInfo)
        {
            BattleLineFormationNode startNode = _splineNodes[curveIndex];
            BattleLineFormationNode endNode = _splineNodes[curveIndex + 1];

            int divisibleNodeCount = curveInfo.nodeCount - 1;
            BattleLineFormationNode currentNode;
            BattleLineFormationNode previousNode;
            Vector3 calculatedPosition;
            Vector3 previousCalculatedPosition = startNode.Position;
            Vector3 displacement;
            int halfNodeCount = curveInfo.nodeCount / 2;
            int nodeIndex;

            previousNode = startNode;
            for (nodeIndex = 1; nodeIndex < halfNodeCount; nodeIndex++)
            {
                calculatedPosition = transform.InverseTransformPoint(GetPosition((float)nodeIndex / divisibleNodeCount, curveIndex));
                currentNode = _battleLineNodes[curveInfo.battleLineIndex + nodeIndex];
                displacement = (calculatedPosition - previousCalculatedPosition).normalized * (previousNode.Radius + currentNode.Radius);

                currentNode.Position = previousNode.Position + displacement;
                previousCalculatedPosition = calculatedPosition;
                previousNode = currentNode;
            }

            previousNode = endNode;
            previousCalculatedPosition = endNode.Position;
            for (nodeIndex = divisibleNodeCount - 1; nodeIndex >= halfNodeCount; nodeIndex--)
            {
                calculatedPosition = transform.InverseTransformPoint(GetPosition((float)nodeIndex / divisibleNodeCount, curveIndex));
                currentNode = _battleLineNodes[curveInfo.battleLineIndex + nodeIndex];
                displacement = (calculatedPosition - previousCalculatedPosition).normalized * (previousNode.Radius + currentNode.Radius);

                currentNode.Position = previousNode.Position + displacement;
                previousCalculatedPosition = calculatedPosition;
                previousNode = currentNode;
            }

            BattleLineFormationNode leftNode;
            BattleLineFormationNode rightNode;
            Vector3 inAnchorOffset = endNode.InAnchor.Position - endNode.Position;
            if (curveInfo.nodeCount % 2 == 1)
            {
                calculatedPosition = transform.InverseTransformPoint(GetPosition(0.5f, curveIndex));
                int midNodeIndex = curveInfo.battleLineIndex + halfNodeCount;
                currentNode = _battleLineNodes[midNodeIndex];
                currentNode.Position = calculatedPosition;
                leftNode = _battleLineNodes[midNodeIndex - 1];
                rightNode = _battleLineNodes[midNodeIndex + 1];
                Vector3 leftNodeDifference = currentNode.Position - leftNode.Position;
                Vector3 rightNodeDifference = rightNode.Position - currentNode.Position;
                Vector3 leftDisplacement = leftNodeDifference.normalized * (leftNode.Radius + currentNode.Radius);
                Vector3 rightDisplacement = rightNodeDifference.normalized * (rightNode.Radius + currentNode.Radius);
                Vector3 leftOffset = leftNodeDifference - leftDisplacement;
                Vector3 rightOffset = rightNodeDifference - rightDisplacement;

                currentNode.Position -= leftOffset;
                Vector3 totalOffset = leftOffset + rightOffset;
                for (nodeIndex = midNodeIndex + 1; nodeIndex < curveInfo.EndNodeBattleLineIndex; nodeIndex++)
                {
                    _battleLineNodes[nodeIndex].Position -= totalOffset;
                }
                endNode.Position -= totalOffset;
            }
            else
            {
                leftNode = _battleLineNodes[halfNodeCount - 1];
                rightNode = _battleLineNodes[halfNodeCount];
                Vector3 midDifference = rightNode.Position - leftNode.Position;
                Vector3 midDisplacement = midDifference.normalized * (leftNode.Radius + rightNode.Radius);
                Vector3 midOffset = midDifference - midDisplacement;

                for (nodeIndex = halfNodeCount; nodeIndex < curveInfo.EndNodeBattleLineIndex; nodeIndex++)
                {
                    _battleLineNodes[nodeIndex].Position -= midOffset;
                }
                endNode.Position -= midOffset;
            }
            endNode.InAnchor.Position = inAnchorOffset + endNode.Position;

            /*
            int divisibleNodeCount = curveInfo.nodeCount - 1;
            BattleLineFormationNode previousNode = startNode;
            Vector3 previousCalculatedPosition = startNode.Position;
            Vector3 displacement;
            int nodeIndex;
            for (nodeIndex = 1; nodeIndex < divisibleNodeCount; nodeIndex++)
            {
                Vector3 calculatedPosition = transform.InverseTransformPoint(GetPosition((float)nodeIndex / divisibleNodeCount, curveIndex));
                BattleLineFormationNode currentNode = _battleLineNodes[curveInfo.battleLineIndex + nodeIndex];
                displacement = (calculatedPosition - previousCalculatedPosition).normalized * (previousNode.Radius + currentNode.Radius);

                currentNode.Position = previousNode.Position + displacement;
                previousCalculatedPosition = calculatedPosition;
                previousNode = currentNode;
            }
            displacement = (endNode.Position - previousCalculatedPosition).normalized * (previousNode.Radius + endNode.Radius);
            Vector3 inAnchorOffset = endNode.InAnchor.Position - endNode.Position;
            endNode.Position = previousNode.Position + displacement;
            endNode.InAnchor.Position = inAnchorOffset + endNode.Position;
            */

            if (!startNode.OutAnchor.IsSameRotation() || !endNode.InAnchor.IsSameRotation())
            {
                Vector3 offset = endNode.Position - endNode.PositionSnapshot;
                for (nodeIndex = curveIndex + 1; nodeIndex < _splineNodes.Count; nodeIndex++)
                {
                    _splineNodes[nodeIndex].Position += offset;
                }
            }
        }

        private bool ReflectCurves(int startNodeIndex)
        {
            int midNodeIndex = startNodeIndex + 1;
            int endNodeIndex = startNodeIndex + 2;

            BattleLineFormationNode startNode = _splineNodes[startNodeIndex];
            BattleLineFormationNode midNode = _splineNodes[midNodeIndex];
            BattleLineFormationNode endNode = _splineNodes[endNodeIndex];

            if (!midNode.IsSamePosition())
            {
                Vector3 midNodePositionDifference = midNode.PositionSnapshot - midNode.Position;
                Vector3 splineSegmentDisplacement = endNode.PositionSnapshot - startNode.Position;
                Vector3 reflection = Vector3.Reflect(midNodePositionDifference, splineSegmentDisplacement.normalized);
                Vector3 updatedEndNodeDisplacement = endNode.PositionSnapshot - midNode.PositionSnapshot + reflection;
                Vector3 updatedEndNodePosition = midNode.Position + updatedEndNodeDisplacement;
                Vector3 offset = updatedEndNodePosition - endNode.Position;

                endNode.Position = updatedEndNodePosition;

                for (int nodeIndex = endNodeIndex + 1; nodeIndex < _splineNodes.Count; nodeIndex++)
                {
                    _splineNodes[nodeIndex].Position += offset;
                }
                return true;
            }
            return false;
        }

        private void SnapshotSplinePoints()
        {
            int curveCount = CurveCount;
            _splineNodes[0].SnapshotPosition();
            _splineNodes[0].OutAnchor.SnapshotRotation();
            _splineNodes[0].OutAnchor.SnapshotPosition();
            for (int nodeIndex = 1; nodeIndex < curveCount; nodeIndex++)
            {
                _splineNodes[nodeIndex].InAnchor.SnapshotRotation();
                _splineNodes[nodeIndex].InAnchor.SnapshotPosition();
                _splineNodes[nodeIndex].SnapshotPosition();
                _splineNodes[nodeIndex].OutAnchor.SnapshotRotation();
                _splineNodes[nodeIndex].OutAnchor.SnapshotPosition();
            }
            _splineNodes[curveCount].InAnchor.SnapshotRotation();
            _splineNodes[curveCount].InAnchor.SnapshotPosition();
            _splineNodes[curveCount].SnapshotPosition();
        }

        private void InitializeSpline()
        {
            BattleLineFormationNode startNode = new BattleLineFormationNode();
            BattleLineFormationAnchor outAnchor = new BattleLineFormationAnchor();
            BattleLineFormationAnchor inAnchor = new BattleLineFormationAnchor();
            BattleLineFormationNode endNode = new BattleLineFormationNode();

            startNode.OutAnchor = outAnchor;
            endNode.InAnchor = inAnchor;

            startNode.Position = new Vector3(1, 0, 0);
            outAnchor.Position = new Vector3(2, 0, 0);
            inAnchor.Position = new Vector3(3, 0, 0);
            endNode.Position = new Vector3(4, 0, 0);

            startNode.SnapshotPosition();
            outAnchor.SnapshotRotation();
            inAnchor.SnapshotRotation();
            endNode.SnapshotPosition();

            _battleLineNodes = new BattleLineFormationNode[2] { startNode, endNode };
            _splineNodes = new List<BattleLineFormationNode>() { startNode, endNode };
            _splineAnchorModes = new List<AnchorMode> { AnchorMode.QUADRATIC };
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