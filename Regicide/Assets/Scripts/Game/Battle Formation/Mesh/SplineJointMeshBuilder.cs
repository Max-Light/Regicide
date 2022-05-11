using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    public class SplineJointMeshBuilder 
    {
        private int[] _endPoints = new int[3];
        private Vector3[] _endPositions = new Vector3[3];

        public int[] EndPoints { get => _endPoints; }
        public Vector3[] EndPositions { get => _endPositions; }

        public void ConstructMesh(CurveSegmentMeshBuilder curve)
        {

        }
    }
}