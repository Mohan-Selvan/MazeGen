using System;
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

        //private 
        private Cell[,] _cells;
        private Coroutine _routine = null;
    

        private void Start()
        {
            _cells = new Cell[gridSize.x, gridSize.y];

            Vector2 basePosition = transform.position;
            for (int i = 0; i < gridSize.x; i++)
            {
                for (int j = 0; j < gridSize.y; j++)
                {
                    Cell cell = new Cell();
                    Vector2 cellPosition = basePosition + (cellSize * i * Vector2.right) + (cellSize * j * Vector2.down);

                    cell.Initialize(new Vector2Int(i, j), cellPosition);
                    _cells[i, j] = cell;
                }
            }
        }

        public void GenerateMaze()
        {
            if(_routine != null)
            {
                Debug.Log("Stopping running coroutine!");
                StopCoroutine(_routine);
            }

            _routine = StartCoroutine(GenerateMaze_Routine());
        }

        private IEnumerator GenerateMaze_Routine()
        {
            Debug.Log("GenerateMaze_Routine : Start");

            WaitForSeconds wait = new WaitForSeconds(0.1f);

            bool[,] visitations = new bool[_cells.GetLength(0), _cells.GetLength(1)];
            for (int i = 0; i < visitations.GetLength(0); i++)
            {
                for (int j = 0; j < visitations.GetLength(1); j++)
                {
                    visitations[i, j] = false;
                }
            }

            Stack<Cell> stack = new Stack<Cell>();


            Cell currentCell = _cells[0, 0];
            visitations[currentCell.GridPosition.x, currentCell.GridPosition.y] = true;
            stack.Push(currentCell);

            while (stack.Count > 0)
            {
                currentCell = stack.Pop();

                List<Cell> unvisitedNeighborCells = GetUnvisitedNeighborCells(currentCell);
                if(unvisitedNeighborCells.Count > 0)
                {
                    stack.Push(currentCell);

                    // TODO :: Random sample
                    Cell nextCell = unvisitedNeighborCells[0];
                    RemoveWallBetweenCells(currentCell, nextCell);
                    visitations[nextCell.GridPosition.x, nextCell.GridPosition.y] = true;
                    stack.Push(nextCell);
                    currentCell = nextCell;
                }

                yield return wait;
                yield return null;
            }

            Debug.Log("GenerateMaze_Routine : Stop");

            List<Cell> GetUnvisitedNeighborCells(Cell cell)
            {
                List<Cell> unvisitedNeighbors = new List<Cell>();

                List<Vector2Int> allNeighborCoords = new List<Vector2Int>()
                {
                    new Vector2Int(cell.GridPosition.x + 0, cell.GridPosition.y + 1),
                    new Vector2Int(cell.GridPosition.x + 1, cell.GridPosition.y + 0),
                    new Vector2Int(cell.GridPosition.x + 0, cell.GridPosition.y - 1),
                    new Vector2Int(cell.GridPosition.x - 1, cell.GridPosition.y + 0),
                };

                foreach (Vector2Int coord in allNeighborCoords)
                {
                    if (IsCoordinateInsideGrid(coord) && (!visitations[coord.x, coord.y]))
                    {
                        unvisitedNeighbors.Add(_cells[coord.x, coord.y]);
                    }
                }

                return unvisitedNeighbors;
            }
        }

        private void RemoveWallBetweenCells(Cell currentCell, Cell nextCell)
        {
            Vector2Int diff = nextCell.GridPosition - currentCell.GridPosition;
            if(diff.x > 0)
            {
                nextCell.SetWall(Wall.LEFT, false);
                currentCell.SetWall(Wall.RIGHT, false);
            }
            else if(diff.x < 0)
            {
                nextCell.SetWall(Wall.RIGHT, false);
                currentCell.SetWall(Wall.LEFT, false);
            }
            else if(diff.y > 0)
            {
                nextCell.SetWall(Wall.BOTTOM, false);
                currentCell.SetWall(Wall.TOP, false);
            }
            else if (diff.y < 0)
            {
                nextCell.SetWall(Wall.TOP, false);
                currentCell.SetWall(Wall.BOTTOM, false);
            }
        }

        private bool IsCoordinateInsideGrid(Vector2Int coord)
        {
            return coord.x >= 0 && coord.x < gridSize.x && coord.y >= 0 && coord.y < gridSize.y;
        }
        #region Gizmos

        private void OnDrawGizmos()
        {
            if (!enableDebugDraw) { return; }
            
            if(_cells == null) { return; }

            Vector2 basePosition = transform.position;

            for (int i = 0; i < _cells.GetLength(0);  i++)
            {
                for (int j = 0; j < _cells.GetLength(1); j++)
                {
                    Cell cell = _cells[i, j];
                    cell.Draw(cell.WorldPosition, cellSize);
                }
            }
        }

        #endregion
    }
}

