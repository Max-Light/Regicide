using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    public class BattleLineMeshBuilder
    {
        private BattleLineSpline _battleLine;
        private Vector3[] _vertices;
        private int[] _triangles;
        private float _radius;
        private float _height;

        private int _vertexIndex = 0;
        private int _triangleIndex = 0;
        private int _pointNumber = 0;

        public BattleLineSpline BattleLine { get => _battleLine; }
        public Vector3[] Vertices { get => _vertices; }
        public int[] Triangles { get => _triangles; }
        public float Radius { get => _radius; }
        public float Height { get => _height; }

        public Transform transform { get => _battleLine.transform; }
        public int VertexIndex { get => _vertexIndex; set => _vertexIndex = value; }
        public int TriangleIndex { get => _triangleIndex; set => _triangleIndex = value; }
        public int PointNumber { get => _pointNumber; set => _pointNumber = value; }

        public BattleLineMeshBuilder(BattleLineSpline battleLine, float radius, float height)
        {
            _battleLine = battleLine;
            _radius = radius;
            _height = height;
            CapMeshBuilder cap = new CapMeshBuilder(this);
            _vertices = new Vector3[CapMeshBuilder.VertexCount];
            _triangles = new int[CapMeshBuilder.TriangleCount];
            cap.ConstructStartCapMesh();
        }
    }
}