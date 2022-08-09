using Regicide.Game.Entity.Navigation;
using System.Collections;
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    public class FormationUnit : MonoBehaviour
    {
        [SerializeField] private float _radius = 1;
        [SerializeField] private PlayerNavigatedAgent _navAgent = null;
        public BattleLineSpline testSpline = null;
        public FormationNode _formationNode = null;

        public float Radius { get => _radius; set => _radius = value; }
        public PlayerNavigatedAgent NavAgent { get => _navAgent; }

        private void Start()
        {
            if (_navAgent != null && !_navAgent.HasPath)
            {
                StartCoroutine(nameof(Track));
            }
        }

        private IEnumerator Track()
        {
            while (true)
            {
                Vector3 position = testSpline.transform.TransformPoint(_formationNode.Position);
                _navAgent.CreatePath(position);

                _navAgent.RotateTowardsPosition(position + Vector3.Cross(_formationNode.Tangent, Vector3.up));
                yield return new WaitForSeconds(1);
            }
        }

        private void OnValidate()
        {
            _navAgent = GetComponent<PlayerNavigatedAgent>();
        }
    }
}