
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    [CustomEditor(typeof(BattleLineFormationSpline))]
    public class BattleLineFormationSplineInspector : Editor
    {
        private const float _anchorSize = 0.04f;
        private const float _anchorPickSize = 0.06f;
        private BattleLineFormationPoint _selectedPoint = null;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }

        private void OnSceneGUI()
        {
            BattleLineFormationSpline battleLine = target as BattleLineFormationSpline;
            
            DrawSplinePoints(battleLine);
            DrawSpline(battleLine);
        }

        private void DrawSpline(BattleLineFormationSpline battleLine)
        {
            IReadOnlyList<BattleLineFormationNode> splineNodes = battleLine.SplineNodes;
            Transform splineTransform = battleLine.transform;

            for (int curveIndex = 0; curveIndex < battleLine.CurveCount; curveIndex++)
            {
                Vector3 p0 = splineTransform.TransformPoint(splineNodes[curveIndex].Position);
                Vector3 p1 = splineTransform.TransformPoint(splineNodes[curveIndex].OutAnchor.Position);
                Vector3 p2 = splineTransform.TransformPoint(splineNodes[curveIndex + 1].InAnchor.Position);
                Vector3 p3 = splineTransform.TransformPoint(splineNodes[curveIndex + 1].Position);

                Handles.color = Color.magenta;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);
                Handles.DrawBezier(p0, p3, p1, p2, Color.blue, null, 2f);
            }
        }

        private void DrawSplinePoints(BattleLineFormationSpline battleLine)
        {
            IReadOnlyList<BattleLineFormationNode> splineNodes = battleLine.SplineNodes;
            if (battleLine.CurveCount > 0)
            {
                int curveCount = battleLine.CurveCount;
                DrawHandlePoint(battleLine, splineNodes[0]);
                DrawHandlePoint(battleLine, splineNodes[0].OutAnchor);
                for (int pointIndex = 1; pointIndex < curveCount; pointIndex++)
                {
                    DrawHandlePoint(battleLine, splineNodes[pointIndex].InAnchor);
                    DrawHandlePoint(battleLine, splineNodes[pointIndex]);
                    DrawHandlePoint(battleLine, splineNodes[pointIndex].OutAnchor);
                }
                DrawHandlePoint(battleLine, splineNodes[curveCount].InAnchor);
                DrawHandlePoint(battleLine, splineNodes[curveCount]);
            }
        }

        private void DrawHandlePoint(BattleLineFormationSpline battleLine, BattleLineFormationPoint point)
        {
            Transform splineTransform = battleLine.transform;
            Vector3 worldPoint = splineTransform.TransformPoint(point.Position);
            Quaternion pointRotation;
            if (Tools.pivotRotation == PivotRotation.Local)
            {
                pointRotation = splineTransform.rotation;
            }
            else
            {
                pointRotation = Quaternion.identity;
            }
            float size = HandleUtility.GetHandleSize(worldPoint);
            Handles.color = Color.green;
            if (Handles.Button(worldPoint, pointRotation, size * _anchorSize, size * _anchorPickSize, Handles.DotHandleCap))
            {
                _selectedPoint = point;
            }
            if (_selectedPoint == point)
            {
                EditorGUI.BeginChangeCheck();
                worldPoint = Handles.DoPositionHandle(worldPoint, pointRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(battleLine, "Move Point");
                    EditorUtility.SetDirty(battleLine);
                    point.Position = splineTransform.InverseTransformPoint(worldPoint);
                }
            }
        }
    }
}