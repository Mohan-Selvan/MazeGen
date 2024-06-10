using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.BlackSpear.MazeGen
{
    public enum Wall { TOP = 0, RIGHT = 1, BOTTOM = 2, LEFT = 3 };

    public class Cell
    {
        public const int SIDES = 4;
        public Vector2 Position = default;

        [SerializeField] List<bool> walls = null;

        internal void Initialize(Vector2 position)
        {
            this.Position = position;

            walls = new List<bool>();

            for (int i = 0; i < SIDES; i++)
            {
                walls.Add(true);
            }
        }

        internal bool GetWall(Wall wall)
        {
            switch (wall)
            {
                case Wall.TOP:
                    return walls[0];
                case Wall.RIGHT:
                    return walls[1];
                case Wall.BOTTOM:
                    return walls[2];
                case Wall.LEFT:
                    return walls[3];
                default:
                    Debug.LogError($"Invalid wall : {wall}");
                    return walls[0];
            }
        }

        internal void Draw(Vector2 position, float cellSize)
        {
            Gizmos.color = Color.white;

            if(GetWall(Wall.TOP))
            {
                Gizmos.DrawLine(position, position + (Vector2.right * cellSize));
            }
            if (GetWall(Wall.RIGHT))
            {
                Gizmos.DrawLine(position + Vector2.right * cellSize, position + (Vector2.down * cellSize));
            }
            if (GetWall(Wall.BOTTOM))
            {
                Gizmos.DrawLine(position + (Vector2.down * cellSize), position + Vector2.down* cellSize);
            }
            if (GetWall(Wall.LEFT))
            {
                Gizmos.DrawLine(position, position + (Vector2.down * cellSize));
            }
        }
    }
}

