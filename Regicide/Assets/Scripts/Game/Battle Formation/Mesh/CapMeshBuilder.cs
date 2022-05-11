using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    public class CapMeshBuilder
    {
        public enum CornerType
        {
            ROUNDED,
            SHARP
        }

        private const int _capRimPartitions = 5;
        private const float _roundedCornerRatio = 0.35f;

        BattleLineMeshBuilder _battleLineMeshBuilder;
        private int[] _endPoints = new int[6];
        private Vector3[] _endPositions = new Vector3[6];

        private CornerType _northCorner = CornerType.ROUNDED;
        private CornerType _southCorner = CornerType.ROUNDED;

        public static int VertexCount { get => 7 + 2 * _capRimPartitions; }
        public static int TriangleCount { get => 6 * (_capRimPartitions + 3); }
        public int[] EndPoints { get => _endPoints; }
        public Vector3[] EndPositions { get => _endPositions; }

        public CapMeshBuilder(BattleLineMeshBuilder battleLineMeshBuilder)
        {
            _battleLineMeshBuilder = battleLineMeshBuilder;
        }

        public void ConstructStartCapMesh()
        {
            Vector3 height = Vector3.up * _battleLineMeshBuilder.Height;
            float rimRadiusFactor = 1 - _roundedCornerRatio;

            int vertexIndex = _battleLineMeshBuilder.VertexIndex;
            int triangleIndex = _battleLineMeshBuilder.TriangleIndex;
            int pointNumber = _battleLineMeshBuilder.PointNumber;

            Vector3 center = _battleLineMeshBuilder.transform.InverseTransformPoint(_battleLineMeshBuilder.BattleLine.GetPosition(0)) + height / 2;
            Vector3 tangent = _battleLineMeshBuilder.BattleLine.GetTangent(0).normalized;
            Vector3 cross = Vector3.Cross(tangent, Vector3.up).normalized * _battleLineMeshBuilder.Radius;

            Vector3 bottomPosition = center - cross;
            Vector3 topPosition = center + cross;

            _battleLineMeshBuilder.Vertices[vertexIndex] = center;
            _battleLineMeshBuilder.Vertices[vertexIndex + 1] = bottomPosition;
            _battleLineMeshBuilder.Vertices[vertexIndex + 2] = topPosition;
            vertexIndex += 3;

            _endPoints[0] = pointNumber;
            _endPoints[1] = pointNumber + 1;
            _endPoints[2] = pointNumber + 2;

            _endPositions[0] = center;
            _endPositions[1] = bottomPosition;
            _endPositions[2] = topPosition;

            Vector3 capOffset = -tangent.normalized * rimRadiusFactor * _battleLineMeshBuilder.Radius;
            center = center + capOffset;
            bottomPosition = bottomPosition + capOffset;
            topPosition = topPosition + capOffset;

            _battleLineMeshBuilder.Vertices[vertexIndex] = center;
            _battleLineMeshBuilder.Vertices[vertexIndex + 1] = bottomPosition;
            _battleLineMeshBuilder.Vertices[vertexIndex + 2] = topPosition;
            vertexIndex += 3;

            _battleLineMeshBuilder.Triangles[triangleIndex] = pointNumber;
            _battleLineMeshBuilder.Triangles[triangleIndex + 1] = pointNumber + 1;
            _battleLineMeshBuilder.Triangles[triangleIndex + 2] = pointNumber + 3;
            _battleLineMeshBuilder.Triangles[triangleIndex + 3] = pointNumber + 3;
            _battleLineMeshBuilder.Triangles[triangleIndex + 4] = pointNumber + 1;
            _battleLineMeshBuilder.Triangles[triangleIndex + 5] = pointNumber + 4;
            _battleLineMeshBuilder.Triangles[triangleIndex + 6] = pointNumber;
            _battleLineMeshBuilder.Triangles[triangleIndex + 7] = pointNumber + 5;
            _battleLineMeshBuilder.Triangles[triangleIndex + 8] = pointNumber + 2;
            _battleLineMeshBuilder.Triangles[triangleIndex + 9] = pointNumber;
            _battleLineMeshBuilder.Triangles[triangleIndex + 10] = pointNumber + 3;
            _battleLineMeshBuilder.Triangles[triangleIndex + 11] = pointNumber + 5;
            int[] rimPoints = new int[3]
            {
                pointNumber + 3,
                pointNumber + 4,
                pointNumber + 5
            };
            triangleIndex += 12;
            pointNumber += 6;

            Vector3 bottomRimCenterOffset = rimRadiusFactor * -cross;
            Vector3 topRimCenterOffset = rimRadiusFactor * cross;
            Vector3 bottomRimCenter = bottomRimCenterOffset + center;
            Vector3 topRimCenter = topRimCenterOffset + center;
            Vector3 bottomRimRadius = -cross - bottomRimCenterOffset;
            Vector3 topRimRadius = cross - topRimCenterOffset;
            float anglePartition;
            int previousPoint;

            anglePartition = 90f / _capRimPartitions;
            previousPoint = rimPoints[1];
            for (int partition = 1; partition <= _capRimPartitions; partition++)
            {
                Quaternion rotation = Quaternion.AngleAxis(anglePartition * partition, Vector3.up);
                _battleLineMeshBuilder.Vertices[vertexIndex] = rotation * bottomRimRadius + bottomRimCenter;
                _battleLineMeshBuilder.Triangles[triangleIndex] = rimPoints[0];
                _battleLineMeshBuilder.Triangles[triangleIndex + 1] = previousPoint;
                _battleLineMeshBuilder.Triangles[triangleIndex + 2] = pointNumber;
                previousPoint = pointNumber;

                vertexIndex++;
                triangleIndex += 3;
                pointNumber++;
            }

            _battleLineMeshBuilder.Vertices[vertexIndex] = (-tangent.normalized * _roundedCornerRatio * _battleLineMeshBuilder.Radius) + center;
            _battleLineMeshBuilder.Triangles[triangleIndex] = rimPoints[0];
            _battleLineMeshBuilder.Triangles[triangleIndex + 1] = previousPoint;
            _battleLineMeshBuilder.Triangles[triangleIndex + 2] = pointNumber;
            int joinPoint = pointNumber;

            vertexIndex++;
            triangleIndex += 3;
            pointNumber++;

            anglePartition = -90f / _capRimPartitions;
            previousPoint = rimPoints[2];
            for (int partition = 1; partition <= _capRimPartitions; partition++)
            {
                Quaternion rotation = Quaternion.AngleAxis(anglePartition * partition, Vector3.up);
                _battleLineMeshBuilder.Vertices[vertexIndex] = rotation * topRimRadius + topRimCenter;
                _battleLineMeshBuilder.Triangles[triangleIndex] = rimPoints[0];
                _battleLineMeshBuilder.Triangles[triangleIndex + 1] = pointNumber;
                _battleLineMeshBuilder.Triangles[triangleIndex + 2] = previousPoint;
                previousPoint = pointNumber;

                vertexIndex++;
                triangleIndex += 3;
                pointNumber++;
            }

            _battleLineMeshBuilder.Triangles[triangleIndex] = rimPoints[0];
            _battleLineMeshBuilder.Triangles[triangleIndex + 1] = joinPoint;
            _battleLineMeshBuilder.Triangles[triangleIndex + 2] = previousPoint;

            triangleIndex += 3;

            Debug.Log(vertexIndex + " --> " + _battleLineMeshBuilder.Vertices.Length);
            Debug.Log(triangleIndex + " --> " + _battleLineMeshBuilder.Triangles.Length);
            Debug.Log(pointNumber);

            _battleLineMeshBuilder.VertexIndex = vertexIndex;
            _battleLineMeshBuilder.TriangleIndex = triangleIndex;
            _battleLineMeshBuilder.PointNumber = pointNumber;
        }

        public void ConstructEndCapMesh(CurveSegmentMeshBuilder curve)
        {

        }

        private void ConstructStartCapVertices()
        {
            int vertexIndex = _battleLineMeshBuilder.VertexIndex;
            Vector3[] vertices = _battleLineMeshBuilder.Vertices;
            float baseWidthFactor = (1 - _roundedCornerRatio);
            Vector3 height = Vector3.up * _battleLineMeshBuilder.Height;
            Vector3 tangent = _battleLineMeshBuilder.BattleLine.GetTangent(0).normalized;
            Vector3 cross = Vector3.Cross(tangent, Vector3.up).normalized * _battleLineMeshBuilder.Radius;
            Vector3 topCenter = _battleLineMeshBuilder.transform.InverseTransformPoint(_battleLineMeshBuilder.BattleLine.GetPosition(0)) + height / 2;
            Vector3 bottomCenter = topCenter - height;
            Vector3 capWidth = -tangent.normalized * baseWidthFactor * _battleLineMeshBuilder.Radius;

            vertices[vertexIndex] = topCenter;
            vertices[vertexIndex + 1] = topCenter - cross;
            vertices[vertexIndex + 2] = topCenter + cross;
            vertices[vertexIndex + 3] = vertices[vertexIndex] + capWidth;
            vertices[vertexIndex + 4] = vertices[vertexIndex + 1] + capWidth;
            vertices[vertexIndex + 5] = vertices[vertexIndex + 2] + capWidth;
            vertices[vertexIndex + 6] = bottomCenter;
            vertices[vertexIndex + 7] = bottomCenter - cross;
            vertices[vertexIndex + 8] = bottomCenter + cross;
            vertices[vertexIndex + 9] = vertices[vertexIndex + 6] + capWidth;
            vertices[vertexIndex + 10] = vertices[vertexIndex + 7] + capWidth;
            vertices[vertexIndex + 11] = vertices[vertexIndex + 8] + capWidth;
            vertexIndex += 12;

            Vector3 rotationCenter;
            Vector3 radius;
            float anglePartition;





            
        }

        /*private int[] ConstructStartCapBaseTriangles()
        {
            int triangleIndex = _battleLineMeshBuilder.TriangleIndex;
            int pointNumber = _battleLineMeshBuilder.PointNumber;

            _battleLineMeshBuilder.Triangles[triangleIndex] = pointNumber;
            _battleLineMeshBuilder.Triangles[triangleIndex + 1] = pointNumber + 1;
            _battleLineMeshBuilder.Triangles[triangleIndex + 2] = pointNumber + 3;
            _battleLineMeshBuilder.Triangles[triangleIndex + 3] = pointNumber + 3;
            _battleLineMeshBuilder.Triangles[triangleIndex + 4] = pointNumber + 1;
            _battleLineMeshBuilder.Triangles[triangleIndex + 5] = pointNumber + 4;
            _battleLineMeshBuilder.Triangles[triangleIndex + 6] = pointNumber;
            _battleLineMeshBuilder.Triangles[triangleIndex + 7] = pointNumber + 5;
            _battleLineMeshBuilder.Triangles[triangleIndex + 8] = pointNumber + 2;
            _battleLineMeshBuilder.Triangles[triangleIndex + 9] = pointNumber;
            _battleLineMeshBuilder.Triangles[triangleIndex + 10] = pointNumber + 3;
            _battleLineMeshBuilder.Triangles[triangleIndex + 11] = pointNumber + 5;
        }*/
    }
}