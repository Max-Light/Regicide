
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    [CustomEditor(typeof(BattleLineSpline))]
    public class BattleLineFormationSplineInspector : Editor
    {
        private const float _anchorSize = 0.04f;
        private const float _anchorPickSize = 0.06f;
        private FormationPoint _selectedPoint = null;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }

        private void OnSceneGUI()
        {
            BattleLineSpline battleLine = target as BattleLineSpline;
            
            DrawSplinePoints(battleLine);
            DrawSpline(battleLine);
        }

        private void DrawSpline(BattleLineSpline battleLine)
        {
            IReadOnlyList<SplineNode> splineNodes = battleLine.SplineNodes;
            IReadOnlyList<SplineAnchorPair> splineAnchors = battleLine.SplineAnchors;
            Transform splineTransform = battleLine.transform;

            for (int curveIndex = 0; curveIndex < battleLine.CurveCount; curveIndex++)
            {
                Vector3 p0 = splineTransform.TransformPoint(splineNodes[curveIndex].Position);
                Vector3 p1 = splineTransform.TransformPoint(splineAnchors[curveIndex].OutAnchor.Position);
                Vector3 p2 = splineTransform.TransformPoint(splineAnchors[curveIndex].InAnchor.Position);
                Vector3 p3 = splineTransform.TransformPoint(splineNodes[curveIndex + 1].Position);

                Handles.color = Color.magenta;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);
                Handles.DrawBezier(p0, p3, p1, p2, Color.blue, null, 2f);
            }
        }

        private void DrawSplinePoints(BattleLineSpline battleLine)
        {
            IReadOnlyList<SplineNode> splineNodes = battleLine.SplineNodes;
            IReadOnlyList<SplineAnchorPair> splineAnchors = battleLine.SplineAnchors;

            if (battleLine.CurveCount > 0)
            {
                int curveIndex;
                int curveCount = battleLine.CurveCount;
                for (curveIndex = 0; curveIndex < curveCount; curveIndex++)
                {
                    DrawHandlePoint(battleLine, splineNodes[curveIndex]);
                    DrawHandlePoint(battleLine, splineAnchors[curveIndex].OutAnchor);
                    DrawHandlePoint(battleLine, splineAnchors[curveIndex].InAnchor);
                }
                DrawHandlePoint(battleLine, splineNodes[curveIndex]);
            }
        }

        private void DrawHandlePoint(BattleLineSpline battleLine, FormationPoint point)
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