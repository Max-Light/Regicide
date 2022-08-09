
using UnityEngine;

namespace Regicide.Game.MapTerrain
{
    [ExecuteInEditMode]
    public class MapEnvironmentBuilder : MonoBehaviour
    {
        [SerializeField] private Terrain[] _terrainTiles = null;
        [SerializeField] private TerrainCollider[] _terrainColliders = null;
        private static MapEnvironmentBuilder _singleton = null;

        public Terrain[] TerrainTiles { get => _terrainTiles; }
        public TerrainCollider[] TerrainColliders { get => _terrainColliders; }

        private void Awake()
        {
            if (_singleton == null)
            {
                _singleton = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void OnEnable()
        {
            MapEnvironment.BuildEnvironmentProperties(this);
            if (Application.IsPlaying(this))
            {
                Destroy(this);
            }
        }
    }
}