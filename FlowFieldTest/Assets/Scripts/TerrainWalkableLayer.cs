using System;
using UnityEngine;

namespace ayy
{
    public class TerrainWalkableLayer : IDisposable
    {
        private GameObject _gameObject = null;
        private Material _material;
        private Texture _dataTexture = null;
        
        private int _width = 0;
        private int _height = 0;

        public TerrainWalkableLayer(int width, int height,GameObject gameObject)
        {
            _width = width;
            _height = height;
            _gameObject = gameObject;
            _material = _gameObject.GetComponent<Renderer>().material;
            
            _dataTexture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
        }
        
        public void Dispose()
        {
            if (_material != null)
            {
                UnityEngine.Object.Destroy(_material);
                _material = null;
            }
            
            UnityEngine.Object.Destroy(_dataTexture);
            _dataTexture = null;
        }
        
        public void RefreshDataTexture(MapPassableData mapPassableData)
        {
            for (int x = 0;x < _width;x++)
            {
                for (int y = 0;y < _height;y++)
                {
                    
                }
            }
        }
        
        
    }
    
}

