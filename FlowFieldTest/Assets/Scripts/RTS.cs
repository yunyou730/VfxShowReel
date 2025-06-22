using System;
using UnityEngine;

namespace ayy
{
    public class RTS : MonoBehaviour
    {
        private MapDataPassable _mapData = null;
        private Texture2D _mapDataTexture = null;
        
        public int _mapWidth = 90;
        public int _mapHeight = 75;

        public GameObject _terrain;
        private TerrainWalkableLayer _terrainWalkableLayer = null;
        
        void Start()
        {
            _mapData = new MapDataPassable(_mapWidth, _mapHeight);
            _terrainWalkableLayer = new TerrainWalkableLayer(_mapWidth, _mapHeight,_terrain);
        }

        private void OnDestroy()
        {
            _mapData?.Dispose();
            _mapData = null;
            
            _terrainWalkableLayer?.Dispose();
            _terrainWalkableLayer = null;
        }

        void Update()
        {
            if (_mapData.Dirty)
            {
                
            }
        }
    }

}

