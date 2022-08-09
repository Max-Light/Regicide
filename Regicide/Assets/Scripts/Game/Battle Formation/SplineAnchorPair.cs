using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    [System.Serializable]
    public class SplineAnchorPair 
    {
        public enum AnchorMode
        {
            LINEAR,
            QUADRATIC,
            CUBIC,
        }

        [SerializeField] private SplineAnchor _outAnchor = null;
        [SerializeField] private SplineAnchor _inAnchor = null;
        [SerializeField] private AnchorMode _anchorMode = AnchorMode.LINEAR;

        public SplineAnchor OutAnchor { get => _outAnchor; }
        public SplineAnchor InAnchor { get => _inAnchor; }
        public AnchorMode Mode { get => _anchorMode; set => _anchorMode = value; }

        public SplineAnchorPair(Vector3 outAnchorPosition, Vector3 inAnchorPosition)
        {
            _outAnchor = new SplineAnchor(outAnchorPosition);
            _inAnchor = new SplineAnchor(inAnchorPosition);
        }

        public SplineAnchorPair(Vector3 outAnchorPosition, Vector3 inAnchorPosition, AnchorMode mode)
        {
            _outAnchor = new SplineAnchor(outAnchorPosition);
            _inAnchor = new SplineAnchor(inAnchorPosition);
            _anchorMode = mode;
        }
    }
}