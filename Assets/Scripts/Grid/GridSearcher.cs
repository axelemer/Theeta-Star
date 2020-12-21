using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSearcher : MonoBehaviour
{
    public enum MovementType { Straight, StraightDiagonal }
    public MovementType movementType;

    public GridMaker grid;

    public float heuristicMultiplier = 1f;

    public List<Vector2Int> Directions
    {
        get
        {
            switch (movementType)
            {
                case MovementType.StraightDiagonal:
                    return diagonalDirs;
                case MovementType.Straight:
                default:
                    return straightDirs;
            }
        }
    }

    public LayerMask blockedMask;

    internal Cell start;
    internal Cell end;

    private List<Vector2Int> straightDirs = new List<Vector2Int>();
    private List<Vector2Int> diagonalDirs = new List<Vector2Int>();
    internal List<Cell> path = new List<Cell>();

    #region Debug
    public Color startColor = Color.cyan;
    public Color endColor = Color.magenta;
    public Color startCheckedColor = Color.green;
    public Color endChekedColor = Color.red;
    private List<Cell> cellsChecked;
    public float waitTime;
    public LineRenderer lineRenderer;
    #endregion

    //En start porque la grid se genera en un Awake.
    void Start()
    {
        straightDirs.Add(Vector2Int.right);
        straightDirs.Add(Vector2Int.down);
        straightDirs.Add(Vector2Int.left);
        straightDirs.Add(Vector2Int.up);

        diagonalDirs.Add(Vector2Int.right);
        diagonalDirs.Add(Vector2Int.right + Vector2Int.down);
        diagonalDirs.Add(Vector2Int.down);
        diagonalDirs.Add(Vector2Int.down + Vector2Int.left);
        diagonalDirs.Add(Vector2Int.left);
        diagonalDirs.Add(Vector2Int.left + Vector2Int.up);
        diagonalDirs.Add(Vector2Int.up);
        diagonalDirs.Add(Vector2Int.up + Vector2Int.right);

        foreach (var cell in grid.cells)
        {
            cell.OnSelect += OnCellSelected;
        }
    }

    private void OnCellSelected(Cell cell)
    {
        //if (!cell.Transitable) return;

        //ClearAll();

        //start = end;
        //end = cell;

        //if (start) start.SetColor(startColor);
        //if (end) end.SetColor(endColor);

        //if (end && start)
        //    Search();
    }


    public void Search()
    {
        if (start == null || end == null) return;

        ClearAll();
        cellsChecked = new List<Cell>();//Debug

        path = ThetaStar.Run(start, Satisfies, GetWightedNeighbours, Heuristic, InSight, EuclideanDist);


        //Debug
        StartCoroutine(PaintRoutine());
    }

    private bool Satisfies(Cell cell)
    {
        cellsChecked.Add(cell);//Debug
        return cell.Equals(end);
    }

    private List<Cell> GetNeighbours(Cell current)
    {
        var neighbours = new List<Cell>();

        for (int i = 0; i < Directions.Count; i++)
        {
            var newPos = current.pos + Directions[i];
            if (!InBounds(newPos)) continue;

            var neighbour = grid.cells[newPos.x, newPos.y];
            var aux1 = grid.cells[newPos.x, current.pos.y];
            var aux2 = grid.cells[current.pos.x, newPos.y];
            if (neighbour.Transitable && aux1.Transitable && aux2.Transitable)
                neighbours.Add(neighbour);
        }

        return neighbours;
    }

    private List<Tuple<Cell, float>> GetWightedNeighbours(Cell current)
    {
        var neighbours = new List<Tuple<Cell, float>>();

        for (int i = 0; i < Directions.Count; i++)
        {
            var newPos = current.pos + Directions[i];
            if (!InBounds(newPos)) continue;

            var neighbour = grid.cells[newPos.x, newPos.y];
            var aux1 = grid.cells[newPos.x, current.pos.y];
            var aux2 = grid.cells[current.pos.x, newPos.y];
            if (neighbour.Transitable && aux1.Transitable && aux2.Transitable)
                neighbours.Add(Tuple.Create(neighbour, Directions[i].magnitude * neighbour.Cost));
        }

        return neighbours;
    }

    private float Heuristic(Cell cell)
    {
        //return EuclideanDist(end, cell);
        return EuclideanDist(end, cell) * heuristicMultiplier;
    }

    private float EuclideanDist(Cell a, Cell b)
    {
        return Vector2Int.Distance(a.pos, b.pos);
    }

    private bool InSight(Cell a, Cell b)
    {
        var start = a.transform.position;
        var dir = (b.transform.position - a.transform.position).normalized;
        var dist = Vector3.Distance(b.transform.position, a.transform.position);
        RaycastHit h;
        var hit = Physics.SphereCast(a.transform.position, 0.5f, dir, out h, dist, blockedMask);
        return !hit;
    }

    private bool InBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < grid.size.x &&
               pos.y >= 0 && pos.y < grid.size.y;
    }

    //Debug
    internal void ClearAll()
    {
        StopAllCoroutines();
        lineRenderer.positionCount = 0;
        foreach (var cell in grid.cells)
        {
            if (cell.Transitable)
                cell.SetColor(cell.defaultColor);
        }
    }

    private void ResetCosts()
    {
        foreach (var cell in grid.cells)
        {
            cell.Cost = 1;
        }
    }

    private IEnumerator PaintRoutine()
    {
        //Se pintan celdas chequeadas
        for (int i = 0; i < cellsChecked.Count; i++)
        {
            var current = cellsChecked[i];
            current.SetColor(Color.Lerp(startCheckedColor, endChekedColor, i / (float)(cellsChecked.Count - 1)));
            yield return new WaitForSeconds(waitTime);
        }

        if (path != null)
        {
            var positions = new List<Vector3>();
            for (int i = 0; i < path.Count; i++)
            {
                var current = path[i];
                current.SetColor(Color.Lerp(startColor, endColor, i / (float)(path.Count - 1)));
                positions.Add(current.transform.position - Vector3.forward * 0.1f + Vector3.up * 0.2f);
                yield return new WaitForSeconds(waitTime);
            }
            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());
        }
    }

    public List<T> Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    private bool showNumbers = false;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.N))
        {
            showNumbers = !showNumbers;
            foreach (var cell in grid.cells) cell.ShowText(showNumbers);
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            start = end = null;
            ClearAll();
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            start = end = null;
            ResetCosts();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Search();
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 1; i < path.Count; i++)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(path[i].transform.position, path[i - 1].transform.position);
        }
    }
}
