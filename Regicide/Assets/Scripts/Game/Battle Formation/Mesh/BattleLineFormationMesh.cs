using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    public class BattleLineFormationMesh : MonoBehaviour
    {
        private class MeshConstructor 
        {
            private Vector3[] _meshVertices;
            private int[] _meshTriangles;

            private int[] _curvePartitions;
            private float[] _startingLerpValues;
            private float[] _endingLerpValues;

            private int _vertexIndex = 0;
            private int _triangleIndex = 0;
            private int _edgeIndex = 0;

            public Vector3[] MeshVertices { get => _meshVertices; }
            public int[] MeshTriangles { get => _meshTriangles; }
            public int[] CurvePartitions { get => _curvePartitions; }
            public float[] StartingLerpValues { get => _startingLerpValues; }
            public float[] EndingLerpValues { get => _endingLerpValues; }
            public int VertexIndex { get => _vertexIndex; set => _vertexIndex = value; }
            public int TriangleIndex { get => _triangleIndex; set => _triangleIndex = value; }
            public int EdgeIndex { get => _edgeIndex; set => _edgeIndex = value; }

            public MeshConstructor(BattleLineSpline spline)
            {
                BattleLineCurve curve;
                SplineNode startNode;
                SplineNode endNode;
                IReadOnlyList<SplineNode> splineNodes = spline.SplineNodes;
                int curveCount = spline.CurveCount;

                _curvePartitions = new int[curveCount];
                _startingLerpValues = new float[curveCount];
                _endingLerpValues = new float[curveCount];

                int totalPartitions = 0;
                for (int curveIndex = 0; curveIndex < curveCount; curveIndex++)
                {
                    startNode = splineNodes[curveIndex];
                    endNode = splineNodes[curveIndex + 1];
                    curve = spline.GetCurveInfo(curveIndex);
                    _curvePartitions[curveIndex] = (curve.nodeCount - 2) * _formationNodePartitions;
                    _startingLerpValues[curveIndex] = startNode.Radius / curve.curveLength;
                    _endingLerpValues[curveIndex] = (curve.curveLength - endNode.Radius) / curve.curveLength;
                    totalPartitions += _curvePartitions[curveIndex];
                }

                _meshVertices = new Vector3[(2 * _splineNodeHalfPartitions * 3) + (totalPartitions * 3 + 3)];
                _meshTriangles = new int[(2 * _splineNodeHalfPartitions * 12) + (totalPartitions * 12)];
            }
        }

        [SerializeField] private MeshFilter _meshFilter = null;
        [SerializeField] private BattleLineSpline _battleLineSpline = null;
        [SerializeField] private float _radius = 1;

        private const int _formationNodePartitions = 5;
        private const int _splineNodeSmoothPartitions = 10;
        private const int _splineNodeHalfPartitions = 3;
        private const int _endCurvePartitions = 20;

        private void ConstructBattleLineMesh()
        {
            int vertexIndex = 0;
            int triangleIndex = 0;
            int edgeIndex = 0;
            int curveIndex;
            int partition;

            int curveCount = _battleLineSpline.CurveCount;
            int[] curvePartitions = new int[curveCount];
            float[] startingLerpValues = new float[curveCount];
            float[] endingLerpValues = new float[curveCount];
            int totalCurvePartitions = 0;


            for (curveIndex = 0; curveIndex < curveCount; curveIndex++)
            {
                BattleLineCurve curve = _battleLineSpline.GetCurveInfo(curveIndex);
                curvePartitions[curveIndex] = (curve.nodeCount - 1) * _formationNodePartitions;
                totalCurvePartitions += curvePartitions[curveIndex];
            }

            Vector3[] meshVertices = new Vector3[(totalCurvePartitions * 3 + 3) + (_endCurvePartitions - 1)];
            int[] meshTriangles = new int[(totalCurvePartitions * 12) + (_endCurvePartitions * 3)];

            float t;
            Vector3 position;
            Vector3 tangent;
            Vector3 cross;
            Vector3 bottomPosition;
            Vector3 topPosition;
            for (curveIndex = 0; curveIndex < curveCount; curveIndex++)
            {
                for (partition = 0; partition < curvePartitions[curveIndex]; partition++)
                {
                    t = partition / (float)curvePartitions[curveIndex];
                    position = transform.InverseTransformPoint(_battleLineSpline.GetPosition(t, curveIndex));
                    tangent = _battleLineSpline.GetTangent(t, curveIndex);
                    cross = Vector3.Cross(tangent, Vector3.up).normalized * _radius;

                    topPosition = position + cross;
                    bottomPosition = position - cross;

                    meshVertices[vertexIndex] = position;
                    meshVertices[vertexIndex + 1] = bottomPosition;
                    meshVertices[vertexIndex + 2] = topPosition;

                    meshTriangles[triangleIndex] = edgeIndex;
                    meshTriangles[triangleIndex + 1] = edgeIndex + 4;
                    meshTriangles[triangleIndex + 2] = edgeIndex + 1;
                    meshTriangles[triangleIndex + 3] = edgeIndex;
                    meshTriangles[triangleIndex + 4] = edgeIndex + 3;
                    meshTriangles[triangleIndex + 5] = edgeIndex + 4;
                    meshTriangles[triangleIndex + 6] = edgeIndex;
                    meshTriangles[triangleIndex + 7] = edgeIndex + 2;
                    meshTriangles[triangleIndex + 8] = edgeIndex + 3;
                    meshTriangles[triangleIndex + 9] = edgeIndex + 3;
                    meshTriangles[triangleIndex + 10] = edgeIndex + 2;
                    meshTriangles[triangleIndex + 11] = edgeIndex + 5;

                    vertexIndex += 3;
                    triangleIndex += 12;
                    edgeIndex += 3;
                }
            }
            position = transform.InverseTransformPoint(_battleLineSpline.GetPosition(1, curveCount - 1));
            tangent = _battleLineSpline.GetTangent(1, curveCount - 1);
            cross = Vector3.Cross(tangent, Vector3.up).normalized * _radius;

            topPosition = position + cross;
            bottomPosition = position - cross;

            meshVertices[vertexIndex] = position;
            meshVertices[vertexIndex + 1] = bottomPosition;
            meshVertices[vertexIndex + 2] = topPosition;
            vertexIndex += 3;

            float anglePartition = 180f / _endCurvePartitions;
            int center = meshTriangles[triangleIndex - 3];
            for (partition = 1; partition < _endCurvePartitions; partition++)
            {
                Quaternion rotation = Quaternion.AngleAxis(anglePartition * partition, Vector3.up);
                Debug.Log(vertexIndex + " --> " + meshVertices.Length);
                Debug.Log(vertexIndex + "   " + partition);
                meshVertices[vertexIndex] = rotation * cross + position;

                meshTriangles[triangleIndex] = edgeIndex + 2;
                meshTriangles[triangleIndex + 1] = edgeIndex + 3;
                meshTriangles[triangleIndex + 2] = center;

                vertexIndex++;
                triangleIndex += 3;
                edgeIndex++;
            }
            meshTriangles[triangleIndex] = edgeIndex + 2;
            meshTriangles[triangleIndex + 1] = center + 1;
            meshTriangles[triangleIndex + 2] = center;
            triangleIndex += 3;

            Debug.Log(vertexIndex + " --> " + meshVertices.Length);
            Debug.Log(triangleIndex + " --> " + meshTriangles.Length);

            _meshFilter.mesh.vertices = meshVertices;
            _meshFilter.mesh.triangles = meshTriangles;
        }

        /*private void ConstructBattleLineMesh()
        {
            MeshConstructor constructor = new MeshConstructor(_battleLineSpline);

            float t;
            Vector3 position;
            Vector3 tangent;
            Vector3 cross;
            Vector3 bottomPosition;
            Vector3 topPosition;

            //
            for (partition = 0; partition < _splineNodeHalfPartitions; partition++)
            {
                t = (partition / (float)_splineNodeHalfPartitions) * startingLerpValues[0];
                position = transform.InverseTransformPoint(_battleLineSpline.GetPosition(t, 0));
                tangent = _battleLineSpline.GetTangent(t, 0);
                cross = Vector3.Cross(tangent, Vector3.up).normalized * _radius;

                topPosition = position + cross;
                bottomPosition = position - cross;

                meshVertices[vertexIndex] = position;
                meshVertices[vertexIndex + 1] = bottomPosition;
                meshVertices[vertexIndex + 2] = topPosition;

                meshTriangles[triangleIndex] = edgeIndex;
                meshTriangles[triangleIndex + 1] = edgeIndex + 4;
                meshTriangles[triangleIndex + 2] = edgeIndex + 1;
                meshTriangles[triangleIndex + 3] = edgeIndex;
                meshTriangles[triangleIndex + 4] = edgeIndex + 3;
                meshTriangles[triangleIndex + 5] = edgeIndex + 4;
                meshTriangles[triangleIndex + 6] = edgeIndex;
                meshTriangles[triangleIndex + 7] = edgeIndex + 2;
                meshTriangles[triangleIndex + 8] = edgeIndex + 3;
                meshTriangles[triangleIndex + 9] = edgeIndex + 3;
                meshTriangles[triangleIndex + 10] = edgeIndex + 2;
                meshTriangles[triangleIndex + 11] = edgeIndex + 5;

                vertexIndex += 3;
                triangleIndex += 12;
                edgeIndex += 3;
            }
            //

            for (curveIndex = 0; curveIndex < curveCount; curveIndex++)
            {
                for (partition = 0; partition < curvePartitions[curveIndex]; partition++)
                {
                    t = (partition / (float)curvePartitions[curveIndex]) * (endingLerpValues[curveIndex] - startingLerpValues[curveIndex]) + startingLerpValues[curveIndex];
                    position = transform.InverseTransformPoint(_battleLineSpline.GetPosition(t, curveIndex));
                    tangent = _battleLineSpline.GetTangent(t, curveIndex);
                    cross = Vector3.Cross(tangent, Vector3.up).normalized * _radius;

                    topPosition = position + cross;
                    bottomPosition = position - cross;

                    meshVertices[vertexIndex] = position;
                    meshVertices[vertexIndex + 1] = bottomPosition;
                    meshVertices[vertexIndex + 2] = topPosition;

                    meshTriangles[triangleIndex] = edgeIndex;
                    meshTriangles[triangleIndex + 1] = edgeIndex + 4;
                    meshTriangles[triangleIndex + 2] = edgeIndex + 1;
                    meshTriangles[triangleIndex + 3] = edgeIndex;
                    meshTriangles[triangleIndex + 4] = edgeIndex + 3;
                    meshTriangles[triangleIndex + 5] = edgeIndex + 4;
                    meshTriangles[triangleIndex + 6] = edgeIndex;
                    meshTriangles[triangleIndex + 7] = edgeIndex + 2;
                    meshTriangles[triangleIndex + 8] = edgeIndex + 3;
                    meshTriangles[triangleIndex + 9] = edgeIndex + 3;
                    meshTriangles[triangleIndex + 10] = edgeIndex + 2;
                    meshTriangles[triangleIndex + 11] = edgeIndex + 5;

                    vertexIndex += 3;
                    triangleIndex += 12;
                    edgeIndex += 3;
                }
                t = endingLerpValues[curveIndex];
                position = transform.InverseTransformPoint(_battleLineSpline.GetPosition(t, curveIndex));
                tangent = _battleLineSpline.GetTangent(t, curveIndex);
                cross = Vector3.Cross(tangent, Vector3.up).normalized * _radius;

                topPosition = position + cross;
                bottomPosition = position - cross;

                meshVertices[vertexIndex] = position;
                meshVertices[vertexIndex + 1] = bottomPosition;
                meshVertices[vertexIndex + 2] = topPosition;

                vertexIndex += 3;

                if (curveIndex != lastCurveIndex)
                {
                    t = 1f;
                    position = transform.InverseTransformPoint(_battleLineSpline.GetPosition(t, curveIndex));
                    Vector3 inTangent = _battleLineSpline.GetTangent(t, curveIndex);
                    Vector3 inTangentCross = Vector3.Cross(inTangent, Vector3.up).normalized * _radius;

                    t = 0f;
                    Vector3 outTangent = _battleLineSpline.GetTangent(t, curveIndex + 1);
                    Vector3 outTangentCross = Vector3.Cross(outTangent, Vector3.up).normalized * _radius;

                    
                }
            }

            Debug.Log(vertexIndex + " --> " + meshVertices.Length);
            Debug.Log(triangleIndex + " --> " + meshTriangles.Length);

            //
            for (partition = 1; partition <= _splineNodeHalfPartitions; partition++)
            {
                t = (partition / (float)_splineNodeHalfPartitions) * (1 - endingLerpValues[lastCurveIndex]) + endingLerpValues[lastCurveIndex];
                position = transform.InverseTransformPoint(_battleLineSpline.GetPosition(t, lastCurveIndex));
                tangent = _battleLineSpline.GetTangent(t, lastCurveIndex);
                cross = Vector3.Cross(tangent, Vector3.up).normalized * _radius;

                topPosition = position + cross;
                bottomPosition = position - cross;

                meshVertices[vertexIndex] = position;
                meshVertices[vertexIndex + 1] = bottomPosition;
                meshVertices[vertexIndex + 2] = topPosition;

                meshTriangles[triangleIndex] = edgeIndex;
                meshTriangles[triangleIndex + 1] = edgeIndex + 4;
                meshTriangles[triangleIndex + 2] = edgeIndex + 1;
                meshTriangles[triangleIndex + 3] = edgeIndex;
                meshTriangles[triangleIndex + 4] = edgeIndex + 3;
                meshTriangles[triangleIndex + 5] = edgeIndex + 4;
                meshTriangles[triangleIndex + 6] = edgeIndex;
                meshTriangles[triangleIndex + 7] = edgeIndex + 2;
                meshTriangles[triangleIndex + 8] = edgeIndex + 3;
                meshTriangles[triangleIndex + 9] = edgeIndex + 3;
                meshTriangles[triangleIndex + 10] = edgeIndex + 2;
                meshTriangles[triangleIndex + 11] = edgeIndex + 5;

                vertexIndex += 3;
                triangleIndex += 12;
                edgeIndex += 3;
            }
            //

            _meshFilter.mesh.vertices = meshVertices;
            _meshFilter.mesh.triangles = meshTriangles;
        }

        private void ConstructFrontCurveCap(MeshConstructor meshConstructor)
        {
            float t;
            Vector3 position;
            Vector3 tangent;
            Vector3 cross;
            Vector3 bottomPosition;
            Vector3 topPosition;

            int vertexIndex = meshConstructor.VertexIndex;
            int triangleIndex = meshConstructor.TriangleIndex;
            int edgeIndex = meshConstructor.EdgeIndex;

            for (int partition = 0; partition < _splineNodeHalfPartitions; partition++)
            {
                t = (partition / (float)_splineNodeHalfPartitions) * startingLerpValues[0];
                position = transform.InverseTransformPoint(_battleLineSpline.GetPosition(t, 0));
                tangent = _battleLineSpline.GetTangent(t, 0);
                cross = Vector3.Cross(tangent, Vector3.up).normalized * _radius;

                topPosition = position + cross;
                bottomPosition = position - cross;

                meshConstructor.MeshVertices[vertexIndex] = position;
                meshConstructor.MeshVertices[vertexIndex + 1] = bottomPosition;
                meshConstructor.MeshVertices[vertexIndex + 2] = topPosition;

                meshConstructor.MeshTriangles[triangleIndex] = edgeIndex;
                meshConstructor.MeshTriangles[triangleIndex + 1] = edgeIndex + 4;
                meshConstructor.MeshTriangles[triangleIndex + 2] = edgeIndex + 1;
                meshConstructor.MeshTriangles[triangleIndex + 3] = edgeIndex;
                meshConstructor.MeshTriangles[triangleIndex + 4] = edgeIndex + 3;
                meshConstructor.MeshTriangles[triangleIndex + 5] = edgeIndex + 4;
                meshConstructor.MeshTriangles[triangleIndex + 6] = edgeIndex;
                meshConstructor.MeshTriangles[triangleIndex + 7] = edgeIndex + 2;
                meshConstructor.MeshTriangles[triangleIndex + 8] = edgeIndex + 3;
                meshConstructor.MeshTriangles[triangleIndex + 9] = edgeIndex + 3;
                meshConstructor.MeshTriangles[triangleIndex + 10] = edgeIndex + 2;
                meshConstructor.MeshTriangles[triangleIndex + 11] = edgeIndex + 5;

                vertexIndex += 3;
                triangleIndex += 12;
                edgeIndex += 3;
            }
        }*/

        private void ConstructCurveSegment(int curveIndex, MeshConstructor meshConstructor)
        {
            float t;
            Vector3 position;
            Vector3 tangent;
            Vector3 cross;
            Vector3 bottomPosition;
            Vector3 topPosition;

            int vertexIndex = meshConstructor.VertexIndex;
            int triangleIndex = meshConstructor.TriangleIndex;
            int edgeIndex = meshConstructor.EdgeIndex;

            for (int partition = 0; partition < meshConstructor.CurvePartitions[curveIndex]; partition++)
            {
                t = (partition / (float)meshConstructor.CurvePartitions[curveIndex]) * (meshConstructor.EndingLerpValues[curveIndex] - meshConstructor.StartingLerpValues[curveIndex]) + meshConstructor.StartingLerpValues[curveIndex];
                position = transform.InverseTransformPoint(_battleLineSpline.GetPosition(t, curveIndex));
                tangent = _battleLineSpline.GetTangent(t, curveIndex);
                cross = Vector3.Cross(tangent, Vector3.up).normalized * _radius;

                topPosition = position + cross;
                bottomPosition = position - cross;

                meshConstructor.MeshVertices[vertexIndex] = position;
                meshConstructor.MeshVertices[vertexIndex + 1] = bottomPosition;
                meshConstructor.MeshVertices[vertexIndex + 2] = topPosition;

                meshConstructor.MeshTriangles[triangleIndex] = edgeIndex;
                meshConstructor.MeshTriangles[triangleIndex + 1] = edgeIndex + 4;
                meshConstructor.MeshTriangles[triangleIndex + 2] = edgeIndex + 1;
                meshConstructor.MeshTriangles[triangleIndex + 3] = edgeIndex;
                meshConstructor.MeshTriangles[triangleIndex + 4] = edgeIndex + 3;
                meshConstructor.MeshTriangles[triangleIndex + 5] = edgeIndex + 4;
                meshConstructor.MeshTriangles[triangleIndex + 6] = edgeIndex;
                meshConstructor.MeshTriangles[triangleIndex + 7] = edgeIndex + 2;
                meshConstructor.MeshTriangles[triangleIndex + 8] = edgeIndex + 3;
                meshConstructor.MeshTriangles[triangleIndex + 9] = edgeIndex + 3;
                meshConstructor.MeshTriangles[triangleIndex + 10] = edgeIndex + 2;
                meshConstructor.MeshTriangles[triangleIndex + 11] = edgeIndex + 5;

                vertexIndex += 3;
                triangleIndex += 12;
                edgeIndex += 3;
            }
            t = meshConstructor.EndingLerpValues[curveIndex];
            position = transform.InverseTransformPoint(_battleLineSpline.GetPosition(t, curveIndex));
            tangent = _battleLineSpline.GetTangent(t, curveIndex);
            cross = Vector3.Cross(tangent, Vector3.up).normalized * _radius;

            topPosition = position + cross;
            bottomPosition = position - cross;

            meshConstructor.MeshVertices[vertexIndex] = position;
            meshConstructor.MeshVertices[vertexIndex + 1] = bottomPosition;
            meshConstructor.MeshVertices[vertexIndex + 2] = topPosition;

            vertexIndex += 3;
        }

        private void ConstructMidSplineNode(SplineNode node, MeshConstructor meshConstructor)
        {

        }

        private void ConstructEndCurveCap()
        {

        }

        private void LateUpdate()
        {
            /*BattleLineMeshBuilder battleLineMesh = new BattleLineMeshBuilder(_battleLineSpline, 1, 1);
            _meshFilter.mesh.vertices = battleLineMesh.Vertices;
            _meshFilter.mesh.triangles = battleLineMesh.Triangles;*/
        }

        private void Start()
        {
            _meshFilter.mesh = new Mesh();
        }

        private void OnValidate()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _battleLineSpline = GetComponent<BattleLineSpline>();
        }

        private void OnDrawGizmosSelected()
        {
            /*Vector3[] vertices = _meshFilter.sharedMesh.vertices;
            for (int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++)
            {
                Gizmos.DrawCube(transform.TransformPoint(vertices[vertexIndex]), new Vector3(0.1f, 0.1f, 0.1f));
            }*/
        }
    }
}