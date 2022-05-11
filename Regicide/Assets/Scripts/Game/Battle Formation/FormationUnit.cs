using Regicide.Game.Entity.Navigation;
using UnityEngine;

namespace Regicide.Game.BattleFormation
{
    public class FormationUnit : MonoBehaviour
    {
        [SerializeField] private float _radius = 1;
        [SerializeField] private PlayerNavigatedAgent _navAgent = null;

        public float Radius { get => _radius; set => _radius = value; }
        public PlayerNavigatedAgent NavAgent { get => _navAgent; }

        private void OnValidate()
        {
            _navAgent = GetComponent<PlayerNavigatedAgent>();
        }
    }
}