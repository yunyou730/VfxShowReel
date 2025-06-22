using System;
using System.Collections.Generic;
using UnityEngine;

namespace ayy
{
    public enum ETileType
    {
        Walkable,
        Obstacle,
    }

    public enum EFlowDirection
    {
        N,      // 上
        S,      // 下
        W,      // 左
        E,      // 右
        NW,     // 左上
        NE,     // 右上
        SW,     // 左下
        SE,     // 右下
        
        Max,
    }
    
    public class MapPassableData : IDisposable
    {
        private int _width;
        private int _height;

        private ETileType[,] _grid = null;
        private Dictionary<string, FlowField> _flowFields = null;

        private bool _dirty = false;
        public bool Dirty { get { return _dirty; } }
        
        public MapPassableData(int width, int height)
        {
            _grid = new ETileType[width, height];
            for (int x = 0; x< _width; x++)
            {
                for (int y = 0;y < _height; y++)
                {
                    _grid[x,y] = ETileType.Walkable;   
                }
            }
        }

        public ETileType[,] GetAllTiles()
        {
            return _grid;
        }

        public void SetTileType(int x,int y,ETileType tileType)
        {
            _grid[x,y] = tileType;
            _dirty = true;
        }

        public ETileType GetTileType(int x, int y)
        {
            return _grid[x,y];
        }
        
        public void Dispose()
        {
            
        }
    }
}

