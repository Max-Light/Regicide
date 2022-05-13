
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    public class BattleLineSpline : MonoBehaviour
    {
        [SerializeField] private List<SplineNode> _splineNodes = new List<SplineNode>();
        [SerializeField] private List<SplineAnchorPair> _splineAnchors = new List<SplineAnchorPair>();
        [SerializeField] private float _maxCurveSegmentAngle = 30f;
        [SerializeField] private float _maxAnchorAngle = 45f;

        private const float _anchorPartition = 1f / 3f;

        public IReadOnlyList<SplineNode> SplineNodes { get => _splineNodes; }
        public IReadOnlyList<SplineAnchorPair> SplineAnchors { get => _splineAnchors; }
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

            SplineNode point0 = _splineNodes[curveIndex];
            SplineNode point1 = _splineNodes[curveIndex + 1];
            SplineAnchorPair anchors = _splineAnchors[curveIndex];

            return transform.TransformPoint(
                Mathf.Pow(difference, 3) * point0.Position +
                3f * Mathf.Pow(difference, 2) * t * anchors.OutAnchor.Position +
                3f * difference * Mathf.Pow(t, 2) * anchors.InAnchor.Position +
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

            SplineNode point0 = _splineNodes[curveIndex];
            SplineNode point1 = _splineNodes[curveIndex + 1];
            SplineAnchorPair anchors = _splineAnchors[curveIndex];

            return transform.TransformPoint(
                3f * Mathf.Pow(difference, 2) * (anchors.OutAnchor.Position - point0.Position) +
                6f * difference * t * (anchors.InAnchor.Position - anchors.OutAnchor.Position) +
                3f * Mathf.Pow(t, 2) * (point1.Position - anchors.InAnchor.Position)
                ) - transform.position;
        }

        public int GetCurveIndexOf(SplineNode splineNode)
        {
            int index = _splineNodes.IndexOf(splineNode);
            if (index == CurveCount)
            {
                return index - 1;
            }
            return index;
        }

        public void ResetBattleLine(params FormationUnit[] formationUnits)
        {
            _splineNodes.Clear();
            _splineAnchors.Clear();
            if (formationUnits.Length > 1)
            {
                int unitIndex = 0;
                float battleLineLength = 0;

                float lengthBetweenUnits = formationUnits[unitIndex].Radius;
                SplineNode startNode = new SplineNode(formationUnits[unitIndex], new Vector3(lengthBetweenUnits, 0, 0));

                FormationNode prevNode = startNode;
                for (unitIndex = 1; unitIndex < formationUnits.Length - 1; unitIndex++)
                {
                    lengthBetweenUnits = formationUnits[unitIndex].Radius + formationUnits[unitIndex - 1].Radius;
                    FormationNode formationNode = new FormationNode(prevNode, formationUnits[unitIndex], prevNode.Position + new Vector3(lengthBetweenUnits, 0, 0));
                    prevNode = formationNode;
                    battleLineLength += lengthBetweenUnits;
                }
                lengthBetweenUnits = formationUnits[unitIndex].Radius + formationUnits[unitIndex - 1].Radius;
                SplineNode endNode = new SplineNode(prevNode, formationUnits[unitIndex], prevNode.Position + new Vector3(lengthBetweenUnits, 0, 0));
                battleLineLength += lengthBetweenUnits;

                startNode.SnapshotPosition();
                endNode.SnapshotPosition();
                _splineNodes.Add(startNode);
                _splineNodes.Add(endNode);

                float anchorDistance = _anchorPartition * battleLineLength;
                SplineAnchorPair curveAnchors = new SplineAnchorPair(startNode.Position + new Vector3(anchorDistance, 0, 0), endNode.Position - new Vector3(anchorDistance, 0, 0));
                _splineAnchors.Add(curveAnchors);

                UpdateBattleLineSpline();
            }
        }

        public void InsertFormationUnit(FormationUnit formationUnit, FormationNode prevNode)
        {

        }

        public void RemoveFormationUnit(FormationUnit formationUnit)
        {

        }

        public SplineNode SetFormationNodeAsSplineNode(FormationNode formationNode)
        {
            if (!(formationNode is SplineNode))
            {
                FormationNode node = formationNode.PrevNode;
                while (!(node is SplineNode))
                {
                    node = node.PrevNode;
                }
                SplineNode startSplineNode = node as SplineNode;
                int curveIndex = _splineNodes.IndexOf(startSplineNode);
                SplineNode midSplineNode = new SplineNode(formationNode, formationNode.FormationUnit, formationNode.Position);
                SplineNode endSplineNode = _splineNodes[curveIndex + 1];
                formationNode.DetachNode();
                _splineNodes.Insert(curveIndex + 1, midSplineNode);

                Vector3 normalizedStartCurveDisplacement = (midSplineNode.Position - startSplineNode.Position).normalized;
                Vector3 normalizedEndCurveDisplacement = (endSplineNode.Position - midSplineNode.Position).normalized;
                BattleLineCurve curveInfo;

                curveInfo = GetCurveInfo(curveIndex);
                float startAnchorDistance = _anchorPartition * curveInfo.curveLength;
                _splineAnchors[curveIndex].OutAnchor.Position = normalizedStartCurveDisplacement * startAnchorDistance + startSplineNode.Position;
                _splineAnchors[curveIndex].InAnchor.Position = -normalizedStartCurveDisplacement * startAnchorDistance + midSplineNode.Position;
                _splineAnchors[curveIndex].OutAnchor.SnapshotPosition();
                _splineAnchors[curveIndex].InAnchor.SnapshotPosition();

                curveInfo = GetCurveInfo(curveIndex + 1);
                float endAnchorDistance = _anchorPartition * curveInfo.curveLength;
                SplineAnchorPair anchorPair = new SplineAnchorPair(normalizedEndCurveDisplacement * endAnchorDistance + midSplineNode.Position, -normalizedEndCurveDisplacement * endAnchorDistance + endSplineNode.Position, _splineAnchors[curveIndex].Mode);
                _splineAnchors.Insert(curveIndex + 1, anchorPair);

                UpdateBattleLineSpline();
                return midSplineNode;
            }
            return formationNode as SplineNode;
        }

        public void RevertSplineNodeAsFormationNode(SplineNode splineNode)
        {

        }

        public BattleLineCurve GetCurveInfo(int curveIndex)
        {
            SplineNode startNode = _splineNodes[curveIndex];
            SplineNode endNode = _splineNodes[curveIndex + 1];

            int nodeCount = 0;
            float curveLength = 0;
            FormationNode node = startNode;

            while (node != endNode)
            {
                ++nodeCount;
                curveLength += node.Radius + node.NextNode.Radius;
                node = node.NextNode;
            }
            ++nodeCount;
            return new BattleLineCurve
            {
                nodeCount = nodeCount,
                curveLength = curveLength,
                startNode = startNode,
                endNode = endNode
            };
        }

        private void UpdateBattleLineSpline()
        {
            int startCurveIndex = 0;
            int curveIndex = 0;
            int curveCount = CurveCount;

            SplineNode startNode = _splineNodes[0];
            BattleLineCurve curveInfo;

            if (startNode.Position != startNode.PreviousPosition)
            {
                SplineNode endNode = _splineNodes[1];
                startNode.Position = ((startNode.PreviousPosition - endNode.PreviousPosition).magnitude * (startNode.Position - endNode.Position).normalized) + endNode.Position;
                if (curveCount > 1)
                {
                    RestrictStartSplineNode(0);
                }
                curveInfo = GetCurveInfo(curveIndex);

                MaintainAnchorRotations(curveIndex);
                RestrictAnchors(curveIndex, curveInfo);
                CompressBattleLineCurve(curveIndex, curveInfo);
                startCurveIndex++;
            }

            for (curveIndex = startCurveIndex; curveIndex < curveCount - 1; curveIndex++)
            {
                curveInfo = GetCurveInfo(curveIndex);

                MaintainCurveDisplacement(curveIndex);
                RestrictMidSplineNode(curveIndex + 1);
                MaintainAnchorRotations(curveIndex);
                RestrictAnchors(curveIndex, curveInfo);
                CompressBattleLineCurve(curveIndex, curveInfo);

                if (ReflectCurves(curveIndex + 1))
                {
                    curveIndex++;
                    curveInfo = GetCurveInfo(curveIndex);

                    MaintainCurveDisplacement(curveIndex);
                    MaintainAnchorRotations(curveIndex);
                    RestrictAnchors(curveIndex, curveInfo);
                    CompressBattleLineCurve(curveIndex, curveInfo);
                }
            }
            if (curveIndex == curveCount - 1)
            {
                curveInfo = GetCurveInfo(curveIndex);

                MaintainCurveDisplacement(curveIndex);
                if (curveCount > 1)
                {
                    RestrictEndSplineNode(curveCount);
                }
                MaintainAnchorRotations(curveIndex);
                RestrictAnchors(curveIndex, curveInfo);
                CompressBattleLineCurve(curveIndex, curveInfo);
            }

            if (startNode.Position != startNode.PreviousPosition)
            {
                Vector3 startNodeOffset = startNode.PreviousPosition - startNode.Position;
                transform.position = transform.TransformPoint(startNode.Position - startNode.PreviousPosition);
                OffsetBattleLine(startNodeOffset);
            }
            SnapshotSplineNodes();
        }

        private void MaintainCurveDisplacement(int curveIndex)
        {
            SplineNode startNode = _splineNodes[curveIndex];
            SplineNode endNode = _splineNodes[curveIndex + 1];

            Vector3 curveDisplacement = endNode.Position - startNode.Position;
            Vector3 curveDisplacementSnapshot = endNode.PreviousPosition - startNode.PreviousPosition;

            if (curveDisplacement == curveDisplacementSnapshot)
            {
                endNode.Position = startNode.Position + curveDisplacementSnapshot;
            }
            else
            {
                endNode.Position = startNode.Position + (curveDisplacement.normalized * curveDisplacementSnapshot.magnitude);
            }
        }

        private void MaintainAnchorRotations(int curveIndex)
        {
            SplineNode startNode = _splineNodes[curveIndex];
            SplineNode endNode = _splineNodes[curveIndex + 1];
            SplineAnchorPair anchors = _splineAnchors[curveIndex];

            Vector3 curveDisplacement = endNode.Position - startNode.Position;
            Vector3 curveDisplacementSnapshot = endNode.PreviousPosition - startNode.PreviousPosition;

            if (curveDisplacement != curveDisplacementSnapshot)
            {
                Vector3 normalizedCurveDisplacement = curveDisplacement.normalized;

                Vector3 outAnchorDisplacementSnapshot = anchors.OutAnchor.PreviousPosition - startNode.PreviousPosition;
                Vector3 inAnchorDisplacementSnapshot = anchors.InAnchor.PreviousPosition - endNode.PreviousPosition;

                Quaternion outAnchorRotation = Quaternion.FromToRotation(curveDisplacementSnapshot, outAnchorDisplacementSnapshot);
                Quaternion inAnchorRotation = Quaternion.FromToRotation(-curveDisplacementSnapshot, inAnchorDisplacementSnapshot);

                anchors.OutAnchor.Position = outAnchorRotation * (normalizedCurveDisplacement * outAnchorDisplacementSnapshot.magnitude) + startNode.Position;
                anchors.InAnchor.Position = inAnchorRotation * -(normalizedCurveDisplacement * inAnchorDisplacementSnapshot.magnitude) + endNode.Position;
            }
        }

        private void RestrictAnchors(int curveIndex, BattleLineCurve curveInfo)
        {
            SplineNode startNode = _splineNodes[curveIndex];
            SplineNode endNode = _splineNodes[curveIndex + 1];
            SplineAnchorPair anchors = _splineAnchors[curveIndex];
            float anchorDistance = _anchorPartition * curveInfo.curveLength;

            Vector3 curveDisplacement = endNode.Position - startNode.Position;
            Vector3 outAnchorDisplacement = anchors.OutAnchor.Position - startNode.Position;
            Vector3 inAnchorDisplacement = anchors.InAnchor.Position - endNode.Position;

            Quaternion outAnchorRotation = Quaternion.FromToRotation(curveDisplacement, outAnchorDisplacement);
            Quaternion inAnchorRotation = Quaternion.FromToRotation(-curveDisplacement, inAnchorDisplacement);

            float outAnchorAngle = Quaternion.Angle(Quaternion.identity, outAnchorRotation);
            if (outAnchorAngle > _maxAnchorAngle)
            {
                outAnchorDisplacement = Quaternion.Slerp(Quaternion.identity, outAnchorRotation, _maxAnchorAngle / outAnchorAngle) * (curveDisplacement.normalized * anchorDistance);
            }

            float inAnchorAngle = Quaternion.Angle(Quaternion.identity, inAnchorRotation);
            if (inAnchorAngle > _maxAnchorAngle)
            {
                inAnchorDisplacement = Quaternion.Slerp(Quaternion.identity, inAnchorRotation, _maxAnchorAngle / inAnchorAngle) * (-curveDisplacement.normalized * anchorDistance);
            }

            switch (anchors.Mode)
            {
                case SplineAnchorPair.AnchorMode.LINEAR:
                    {
                        outAnchorDisplacement = curveDisplacement;
                        inAnchorDisplacement = -curveDisplacement;
                        break;
                    }
                case SplineAnchorPair.AnchorMode.QUADRATIC:
                    {
                        Vector3 curveDisplacementSnapshot = endNode.PreviousPosition - startNode.PreviousPosition;

                        Vector3 outAnchorDisplacementSnapshot = anchors.OutAnchor.PreviousPosition - startNode.PreviousPosition;
                        Vector3 inAnchorDisplacementSnapshot = anchors.InAnchor.PreviousPosition - endNode.PreviousPosition;

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
                case SplineAnchorPair.AnchorMode.CUBIC:
                    {
                        break;
                    }
            }

            outAnchorDisplacement = outAnchorDisplacement.normalized * anchorDistance;
            inAnchorDisplacement = inAnchorDisplacement.normalized * anchorDistance;

            anchors.OutAnchor.Position = startNode.Position + outAnchorDisplacement;
            anchors.InAnchor.Position = endNode.Position + inAnchorDisplacement;
        }

        private void RestrictStartSplineNode(int startNodeIndex)
        {
            SplineNode startNode = _splineNodes[startNodeIndex];
            SplineNode midNode = _splineNodes[startNodeIndex + 1];
            SplineNode endNode = _splineNodes[startNodeIndex + 2];

            Vector3 startCurveDisplacement = midNode.Position - startNode.Position;
            Vector3 startCurveDisplacementSnapshot = midNode.PreviousPosition - startNode.PreviousPosition;

            if (startCurveDisplacement != startCurveDisplacementSnapshot)
            {
                Vector3 endCurveDisplacement = midNode.Position - endNode.Position;
                Quaternion curveRotation = Quaternion.FromToRotation(endCurveDisplacement, -startCurveDisplacement);
                float curveSegmentAngle = Quaternion.Angle(Quaternion.identity, curveRotation) / 2;

                if (curveSegmentAngle > _maxCurveSegmentAngle)
                {
                    curveRotation = Quaternion.Slerp(Quaternion.identity, curveRotation, _maxCurveSegmentAngle / curveSegmentAngle);
                    startNode.Position = curveRotation * (endCurveDisplacement.normalized * startCurveDisplacement.magnitude) + midNode.Position;
                }
            }
        }

        private void RestrictEndSplineNode(int endNodeIndex)
        {
            SplineNode startNode = _splineNodes[endNodeIndex - 2];
            SplineNode midNode = _splineNodes[endNodeIndex - 1];
            SplineNode endNode = _splineNodes[endNodeIndex];

            Vector3 endCurveDisplacement = midNode.Position - endNode.Position;
            Vector3 endCurveDisplacementSnapshot = midNode.PreviousPosition - endNode.PreviousPosition;

            if (endCurveDisplacement != endCurveDisplacementSnapshot)
            {
                Vector3 startCurveDisplacement = midNode.Position - startNode.Position;
                Quaternion curveRotation = Quaternion.FromToRotation(startCurveDisplacement, -endCurveDisplacement);
                float curveSegmentAngle = Quaternion.Angle(Quaternion.identity, curveRotation) / 2;

                if (curveSegmentAngle > _maxCurveSegmentAngle)
                {
                    curveRotation = Quaternion.Slerp(Quaternion.identity, curveRotation, _maxCurveSegmentAngle / curveSegmentAngle);
                    endNode.Position = curveRotation * (startCurveDisplacement.normalized * endCurveDisplacement.magnitude) + midNode.Position;
                }
            }
        }

        private void RestrictMidSplineNode(int midNodeIndex)
        {
            SplineNode startNode = _splineNodes[midNodeIndex - 1];
            SplineNode midNode = _splineNodes[midNodeIndex];
            SplineNode endNode = _splineNodes[midNodeIndex + 1];

            Vector3 startCurveDisplacement = midNode.Position - startNode.Position;
            Vector3 endCurveDisplacement = midNode.Position - endNode.Position;

            Vector3 startCurveDisplacementSnapshot = midNode.PreviousPosition - startNode.PreviousPosition;
            Vector3 endCurveDisplacementSnapshot = midNode.PreviousPosition - endNode.PreviousPosition;

            if (startCurveDisplacement != startCurveDisplacementSnapshot && endCurveDisplacement != endCurveDisplacementSnapshot)
            {
                Vector3 curveSegmentDisplacement = endNode.Position - startNode.Position;
                Quaternion curveRotation = Quaternion.FromToRotation(curveSegmentDisplacement, startCurveDisplacement);
                float curveSegmentAngle = Quaternion.Angle(Quaternion.identity, curveRotation);

                if (curveSegmentAngle > _maxCurveSegmentAngle)
                {
                    curveRotation = Quaternion.Slerp(Quaternion.identity, curveRotation, _maxCurveSegmentAngle / curveSegmentAngle);
                    startCurveDisplacement = curveSegmentDisplacement.normalized * startCurveDisplacement.magnitude;
                    midNode.Position = curveRotation * startCurveDisplacement + startNode.Position;
                }
            }
        }

        private void CompressBattleLineCurve(int curveIndex, BattleLineCurve curveInfo)
        {
            SplineNode startNode = _splineNodes[curveIndex];
            SplineNode endNode = _splineNodes[curveIndex + 1];
            SplineAnchorPair anchors = _splineAnchors[curveIndex];

            int divisibleNodeCount = curveInfo.nodeCount - 1;
            int nodeIndex;
            Vector3 previousCalculatedPosition;
            Vector3 calculatedPosition;

            Vector3 inAnchorDisplacement = anchors.InAnchor.Position - endNode.Position;
            Vector3 normalizedCurveDisplacement = (endNode.Position - startNode.Position).normalized;
            endNode.Position = (normalizedCurveDisplacement * curveInfo.curveLength) + startNode.Position;
            anchors.InAnchor.Position = endNode.Position + inAnchorDisplacement;

            previousCalculatedPosition = startNode.Position;
            float curveLength = 0;
            for (nodeIndex = 1; nodeIndex < curveInfo.nodeCount; nodeIndex++)
            {
                calculatedPosition = transform.InverseTransformPoint(GetPosition((float)nodeIndex / divisibleNodeCount, curveIndex));
                curveLength += (calculatedPosition - previousCalculatedPosition).magnitude;
                previousCalculatedPosition = calculatedPosition;
            }
            float curveRatio = curveInfo.curveLength / curveLength;
            endNode.Position = (normalizedCurveDisplacement * (curveRatio * curveInfo.curveLength)) + startNode.Position;
            anchors.InAnchor.Position = endNode.Position + inAnchorDisplacement;

            float lengthBetweenNodes;
            previousCalculatedPosition = startNode.Position;
            FormationNode node = startNode.NextNode;
            nodeIndex = 1;
            while (node != endNode)
            {
                calculatedPosition = transform.InverseTransformPoint(GetPosition((float) nodeIndex / divisibleNodeCount, curveIndex));
                lengthBetweenNodes = node.PrevNode.Radius + node.Radius;
                node.Position = (calculatedPosition - previousCalculatedPosition).normalized* lengthBetweenNodes + node.PrevNode.Position;
                previousCalculatedPosition = calculatedPosition;
                node = node.NextNode;
                nodeIndex++;
            }
        }

        private bool ReflectCurves(int mideNodeIndex)
        {
            SplineNode startNode = _splineNodes[mideNodeIndex - 1];
            SplineNode midNode = _splineNodes[mideNodeIndex];
            SplineNode endNode = _splineNodes[mideNodeIndex + 1];

            Vector3 startCurveDisplacement = midNode.Position - startNode.Position;
            Vector3 endCurveDisplacement = endNode.Position - midNode.Position;
            Vector3 startCurveDisplacementSnapshot = midNode.PreviousPosition - startNode.PreviousPosition;
            Vector3 endCurveDisplacementSnapshot = endNode.PreviousPosition - midNode.PreviousPosition;

            if (startCurveDisplacement != startCurveDisplacementSnapshot && endCurveDisplacement != endCurveDisplacementSnapshot)
            {
                Vector3 midNodePositionDifference = startCurveDisplacementSnapshot - startCurveDisplacement;
                Vector3 splineSegmentDisplacement = endNode.Position - startNode.Position;
                Vector3 reflection = Vector3.Reflect(midNodePositionDifference, splineSegmentDisplacement.normalized);
                Vector3 updatedEndNodeDisplacement = endCurveDisplacementSnapshot + reflection;
                Vector3 updatedEndNodePosition = midNode.Position + updatedEndNodeDisplacement;
                
                endNode.Position = updatedEndNodePosition;

                return true;
            }
            return false;
        }

        private void SnapshotSplineNodes()
        {
            int nodeIndex;
            for (nodeIndex = 0; nodeIndex < CurveCount; nodeIndex++)
            {
                _splineAnchors[nodeIndex].OutAnchor.SnapshotPosition();
                _splineAnchors[nodeIndex].InAnchor.SnapshotPosition();
                _splineNodes[nodeIndex].SnapshotPosition();
            }
            _splineNodes[nodeIndex].SnapshotPosition();
        }

        private void OffsetBattleLine(Vector3 offset)
        {
            FormationNode node = _splineNodes[0];
            int curveCount = CurveCount;

            while (node != _splineNodes[curveCount])
            {
                node.Position += offset;
                node = node.NextNode;
            }
            node.Position += offset;
            for (int anchorIndex = 0; anchorIndex < curveCount; anchorIndex++)
            {
                _splineAnchors[anchorIndex].OutAnchor.Position += offset;
                _splineAnchors[anchorIndex].InAnchor.Position += offset;
            }
        }

        private void Update()
        {
            if (CurveCount > 0)
            {
                UpdateBattleLineSpline();
                
                for (int i = 0; i < CurveCount; i++)
                {
                    BattleLineCurve curve = GetCurveInfo(i);
                    FormationNode n = _splineNodes[i];
                    for (int node = 0; node < curve.nodeCount - 1; node++)
                    {
                        
                        n.Tangent = GetTangent(node / curve.nodeCount, i);
                        n = n.NextNode;
                    }
                }
                _splineNodes[CurveCount - 1].Tangent = GetTangent(1, CurveCount - 1);
            }
        }

        private void OnDrawGizmosSelected()
        {
            int curveCount = CurveCount;
            if (curveCount > 0)
            {
                FormationNode node = _splineNodes[0];
                while (node != _splineNodes[curveCount])
                {
                    if (node is SplineNode)
                    {
                        Gizmos.color = Color.yellow;
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                    }
                    Gizmos.DrawSphere(transform.TransformPoint(node.Position), node.Radius);
                    node = node.NextNode;
                }
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(transform.TransformPoint(node.Position), node.Radius);
            }
        }
    }
}