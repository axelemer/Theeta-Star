using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frame : MonoBehaviour
{

    public Cell cellPrefab;
    public Vector2Int size = new Vector2Int(10, 10);
    public float tileSize = 1f;

    public Cell[,] cells;

    private void Awake()
    {
        cells = new Cell[size.x, size.y];

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                var cell = Instantiate(cellPrefab);

                cell.transform.parent = transform;
                cell.transform.localPosition = new Vector3(i * tileSize, 0, j * tileSize);

                cell.pos = new Vector2Int(i, j);
                cells[i, j] = cell;
            }
        }
    }

}
