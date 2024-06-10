using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.BlackSpear.MazeGen
{
    public class MazeGenerator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] int randomSeed = 2;
        [SerializeField] Vector2Int gridSize = Vector2Int.one;
        [SerializeField] float cellSize = 1f;

        [Header("Debug Draw")]
        [SerializeField] bool enableDebugDraw = true;
        [SerializeField] bool enableVisitationDraw = true;

        [Header("Settings")]
        [SerializeField] KeyCode generateKeyCode = KeyCode.Space;
        [SerializeField] float waitDuration = 0.1f;

        //private 
        private Cell[,] _cells;
        private Coroutine _routine = null;
        private bool[,] _visitations = null;
        [SerializeField] private Cell _currentCell = null;
        private System.Random random = null;


        private void Start()
        {
            InitializeGrid();
        }

        private void Update()
        {
            if(Input.GetKeyDown(generateKeyCode))
            {
                InitializeGrid();
                GenerateMaze();
            }
        }

        private void InitializeGrid()
        {
            _cells = new Cell[gridSize.x, gridSize.y];

            Vector2 basePosition = transform.position;
            for (int i = 0; i < gridSize.x; i++)
            {
                for (int j = 0; j < gridSize.y; j++)
                {
                    Cell cell = new Cell();
                    Vector2 cellPosition = basePosition + (cellSize * i * Vector2.right) + (cellSize * j * Vector2.up);

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

            random = new System.Random(Seed: randomSeed);
            WaitForSeconds wait = new WaitForSeconds(waitDuration);

            _visitations = new bool[_cells.GetLength(0), _cells.GetLength(1)];
            for (int i = 0; i < _visitations.GetLength(0); i++)
            {
                for (int j = 0; j < _visitations.GetLength(1); j++)
                {
                    _visitations[i, j] = false;
                }
            }

            Stack<Cell> stack = new Stack<Cell>();


            _currentCell = _cells[0, 0];
            _visitations[_currentCell.GridPosition.x, _currentCell.GridPosition.y] = true;
            stack.Push(_currentCell);

            while (stack.Count > 0)
            {
                _currentCell = stack.Pop();

                List<Cell> unvisitedNeighborCells = GetUnvisitedNeighborCells(_currentCell);
                if(unvisitedNeighborCells.Count > 0)
                {
                    stack.Push(_currentCell);

                    Cell nextCell = unvisitedNeighborCells[random.Next(0, unvisitedNeighborCells.Count)];
                    RemoveWallBetweenCells(_currentCell, nextCell);
                    _visitations[nextCell.GridPosition.x, nextCell.GridPosition.y] = true;
                    stack.Push(nextCell);
                    _currentCell = nextCell;
                }

                yield return wait;
                yield return null;
            }

            Debug.Log("GenerateMaze_Routine : Stop");
            _routine = null;
        }

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
                if (IsCoordinateInsideGrid(coord) && (!_visitations[coord.x, coord.y]))
                {
                    unvisitedNeighbors.Add(_cells[coord.x, coord.y]);
                }
            }

            return unvisitedNeighbors;
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

            for (int i = 0; i < _cells.GetLength(0);  i++)
            {
                for (int j = 0; j < _cells.GetLength(1); j++)
                {
                    Cell cell = _cells[i, j];
                    cell.Draw(cellSize);
                }
            }

            if(enableVisitationDraw && _visitations != null)
            {
                Gizmos.color = Color.yellow;

                for (int i = 0; i < _cells.GetLength(0); i++)
                {
                    for (int j = 0; j < _cells.GetLength(1); j++)
                    {
                        Cell cell = _cells[i, j];
                        
                        if (_visitations[i, j])
                        {
                            Gizmos.DrawCube(cell.WorldPosition + new Vector2(0.5f * cellSize, 0.5f * cellSize), Vector2.one * 0.8f * cellSize);
                        }
                    }
                }
            }

            if(_currentCell != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(_currentCell.WorldPosition + new Vector2(0.5f * cellSize, 0.5f * cellSize), Vector2.one * 0.75f * cellSize);
            }
        }

        #endregion
    }
}

