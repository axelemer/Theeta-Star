using System;
using System.IO;
using UnityEngine;

public class GridMaker : MonoBehaviour
{

    public Cell cellPrefab;
    public Vector2Int size = new Vector2Int(10, 10);
    public float tileSize = 1f;

    public GridScriptableObject scriptableObject;
    public Cell[,] cells;
    internal bool gridCreated;

    private void Awake()
    {
        //string dataCells = File.ReadAllText(Application.persistentDataPath + "/SavedCells.json");
        //if (!string.IsNullOrEmpty(dataCells))
        //{
        //    Cell[] cells1D = JsonUtility.FromJson<Cell[]>(dataCells);
        //    cells = this.To2DArray(cells1D, size.x, size.y);
        //    gridCreated = scriptableObject.gridCreated;
        //    cells = scriptableObject.cells;
        //}

        if (cells == null && !gridCreated)
        {
            cells = new Cell[size.x, size.y];
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    Cell cell = new Cell();
                    if (cells[i, j] != null)
                        cell = cells[i, j];
                    else
                        cell = Instantiate(cellPrefab);

                    cell.gameObject.name = "Cell-" + i + "," + j;
                    cell.transform.parent = transform;
                    cell.transform.localPosition = new Vector3(i * tileSize, 0, j * tileSize);
                    cell.SetPosition();
                    cell.pos = new Vector2Int(i, j);
                    cells[i, j] = cell;
                }
            }
        }

    }


    [ExecuteInEditMode]
    public void DestroyGrid()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (cells != null)
                {
                    if (cells[i, j] != null)
                    {
                        Destroy(cells[i, j]);
                    }
                }
            }
        }


    }

    [ExecuteInEditMode]
    public void CreateGrid()
    {
        cells = new Cell[size.x, size.y];

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                var cell = Instantiate(cellPrefab);

                cell.transform.parent = transform;
                cell.transform.localPosition = new Vector3(i * tileSize, 0, j * tileSize);
                cell.SetPosition();
                cell.pos = new Vector2Int(i, j);
                cells[i, j] = cell;
                cell.ManualReset();
            }
        }

        scriptableObject.cells = cells;
        scriptableObject.gridCreated = true;
        Cell[] cells1D = this.To1DArray(cells);
        string jsonCells = JsonUtility.ToJson(cells1D);
        File.WriteAllText(Application.persistentDataPath + "/SavedCells.json", jsonCells);
    }



    private T[] To1DArray<T>(T[,] matrix)
    {
        int x = matrix.GetLength(0);
        int y = matrix.GetLength(1);

        T[] arr = new T[x * y];

        for (int k = 0; k < x; k++)
        {
            for (int j = 0; j < y; j++)
            {
                arr[k * x + j] = matrix[k, j];
            }
        }
        return arr;
    }

    private T[,] To2DArray<T>(T[] matrix, int x, int y)
    {
        T[,] arr = new T[x, y];

        for (int k = 0; k < x; k++)
        {
            for (int j = 0; j < y; j++)
            {
                arr[k, j] = matrix[k * x + j];
            }
        }
        return arr;
    }

}
