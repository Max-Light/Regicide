
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.MapTerrain
{
    public static class MapEnvironment 
    {
        private static HashSet<Terrain> _terrainTiles = new HashSet<Terrain>();
        private static HashSet<TerrainCollider> _terrainColliders = new HashSet<TerrainCollider>();

        public static HashSet<Terrain> TerrainTiles { get => _terrainTiles; }
        public static HashSet<TerrainCollider> TerrainColliders { get => _terrainColliders; }

        public static void BuildEnvironmentProperties(MapEnvironmentBuilder builder)
        {
            foreach (Terrain terrainTile in builder.TerrainTiles)
            {
                _terrainTiles.Add(terrainTile);
            }
            foreach (TerrainCollider collider in builder.TerrainColliders)
            {
                _terrainColliders.Add(collider);
            }
        }

        public static bool IsTerrainCollider(Collider collider)
        {
            if (collider is TerrainCollider) 
            {
                return _terrainColliders.Contains(collider as TerrainCollider);
            }
            return false;
        }
    }
}