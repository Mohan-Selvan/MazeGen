using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.BlackSpear.MazeGen
{
    public class MazeGenerator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] Vector2Int gridSize = Vector2Int.one;
        [SerializeField] float cellSize = 1f;

        [Header("Debug Draw")]
        [SerializeField] bool enableDebugDraw = false;
        [SerializeField] Color lineColor = Color.white;

        //Helpers
        Cell[,] cells;

        private void Start()
        {
            cells = new Cell[gridSize.x, gridSize.y];

            Vector2 basePosition = transform.position;
            for (int i = 0; i < gridSize.x; i++)
            {
                for (int j = 0; j < gridSize.y; j++)
                {
                    Cell cell = new Cell();
                    Vector2 cellPosition = basePosition + (Vector2.right * i * cellSize) + (Vector2.down * j * cellSize);

                    cell.Initialize(cellPosition);
                    cells[i, j] = cell;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!enableDebugDraw) { return; }
            
            if(cells == null) { return; }

            Vector2 basePosition = transform.position;

            for (int i = 0; i < cells.GetLength(0);  i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    Cell cell = cells[i, j];
                    cell.Draw(cell.Position, cellSize);
                }
            }
        }
    }
}

