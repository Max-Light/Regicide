using System;
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    public class CurveSegmentMeshBuilder
    {
        private const int _curvePartitions = 50;

        private BattleLineMeshBuilder _battleLineMeshBuilder;
        private int[] _endPoints = new int[3];
        private Vector3[] _endPositions = new Vector3[3];

        public static int VertexCount { get => _curvePartitions * 3; }
        public static int TriangleCount { get => _curvePartitions * 12; }
        public int[] EndPoints { get => _endPoints; }
        public Vector3[] EndPositions { get => _endPositions; }

        public CurveSegmentMeshBuilder(BattleLineMeshBuilder battleLineMeshBuilder)
        {
            _battleLineMeshBuilder = battleLineMeshBuilder;
        }

        public void ConstructMesh(CapMeshBuilder startCap, int curveIndex)
        {
            ExtendMesh(startCap.EndPoints, startCap.EndPositions, curveIndex);
        }

        public void ConstructMesh(SplineJointMeshBuilder curve, int curveIndex)
        {
            ExtendMesh(curve.EndPoints, curve.EndPositions, curveIndex);
        }

        private void ExtendMesh(int[] endPoints, Vector3[] endPositions, int curveIndex)
        {
            BattleLineSpline spline = _battleLineMeshBuilder.BattleLine;
            Transform transform = _battleLineMeshBuilder.BattleLine.transform;
            BattleLineCurve curve = _battleLineMeshBuilder.BattleLine.GetCurveInfo(curveIndex);
            float startT = curve.startNode.Radius / curve.curveLength;
            float endT = 1 - (curve.endNode.Radius / curve.curveLength);

            float t;
            Vector3 position = endPositions[0];
            Vector3 topPosition = endPositions[2];
            Vector3 bottomPosition = endPositions[1];
            Vector3 updatedPosition;
            Vector3 positionalDifference;

            int vertexIndex = _battleLineMeshBuilder.VertexIndex;
            int triangleIndex = _battleLineMeshBuilder.TriangleIndex;
            int pointNumber = _battleLineMeshBuilder.PointNumber;

            int[] points = new int[6];
            Array.Copy(endPoints, points, 3);
            points[3] = pointNumber + 3;
            points[4] = pointNumber + 4;
            points[5] = pointNumber + 5;

            for (int partition = 1; partition < _curvePartitions; partition++)
            {
                t = (endT - startT) * (partition / (float)_curvePartitions) + startT;
                updatedPosition = transform.InverseTransformPoint(spline.GetPosition(t, curveIndex));
                positionalDifference = updatedPosition - position;

                position = updatedPosition;
                bottomPosition += positionalDifference;
                topPosition += positionalDifference;

                _battleLineMeshBuilder.Vertices[vertexIndex] = position;
                _battleLineMeshBuilder.Vertices[vertexIndex + 1] = bottomPosition;
                _battleLineMeshBuilder.Vertices[vertexIndex + 2] = topPosition;

                _battleLineMeshBuilder.Triangles[triangleIndex] = points[0];
                _battleLineMeshBuilder.Triangles[triangleIndex + 1] = points[4];
                _battleLineMeshBuilder.Triangles[triangleIndex + 2] = points[1];
                _battleLineMeshBuilder.Triangles[triangleIndex + 3] = points[0];
                _battleLineMeshBuilder.Triangles[triangleIndex + 4] = points[3];
                _battleLineMeshBuilder.Triangles[triangleIndex + 5] = points[4];
                _battleLineMeshBuilder.Triangles[triangleIndex + 6] = points[0];
                _battleLineMeshBuilder.Triangles[triangleIndex + 7] = points[2];
                _battleLineMeshBuilder.Triangles[triangleIndex + 8] = points[3];
                _battleLineMeshBuilder.Triangles[triangleIndex + 9] = points[3];
                _battleLineMeshBuilder.Triangles[triangleIndex + 10] = points[2];
                _battleLineMeshBuilder.Triangles[triangleIndex + 11] = points[5];

                vertexIndex += 3;
                triangleIndex += 12;
                pointNumber += 3;

                points[0] = points[3];
                points[1] = points[4];
                points[2] = points[5];
                points[3] = pointNumber + 3;
                points[4] = pointNumber + 4;
                points[5] = pointNumber + 5;
            }
            t = endT;
            updatedPosition = transform.InverseTransformPoint(spline.GetPosition(t, curveIndex));
            positionalDifference = updatedPosition - position;

            position = updatedPosition;
            bottomPosition += positionalDifference;
            topPosition += positionalDifference;

            _battleLineMeshBuilder.Vertices[vertexIndex] = position;
            _battleLineMeshBuilder.Vertices[vertexIndex + 1] = bottomPosition;
            _battleLineMeshBuilder.Vertices[vertexIndex + 2] = topPosition;

            _battleLineMeshBuilder.VertexIndex = vertexIndex;
            _battleLineMeshBuilder.TriangleIndex = triangleIndex;
            _battleLineMeshBuilder.PointNumber = pointNumber;

            _endPoints[0] = points[3];
            _endPoints[1] = points[4];
            _endPoints[2] = points[5];

            _endPositions[0] = position;
            _endPositions[1] = bottomPosition;
            _endPositions[2] = topPosition;
        }
    }
}