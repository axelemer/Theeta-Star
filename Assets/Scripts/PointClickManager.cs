using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointClickManager : MonoBehaviour
{
    public CharacterMovement character;
    public GridSearcher gridSearcher;
    private Cell currentCell;
    private int currentIndex;
    public float distanceToNextNode;
    public float searchNodeRadius;
    public LayerMask cellsMask;


    void Update()
    {
        CheckClicks();
    }

    private void CheckClicks()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Clicked();
        }

        if(gridSearcher.path.Count > 0)
        {
            SetPathToCharacter();
        }
    }

    private void SetPathToCharacter()
    {
        if(Vector3.Distance(character.transform.position, currentCell.transform.position) < distanceToNextNode)
        {
            if (currentCell != gridSearcher.end)
            {
                currentCell = gridSearcher.path[currentIndex++];
                character.SetPoint(currentCell.transform.position);
            }
        }
    }

    void Clicked()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {
            bool checkCells = Physics.CheckSphere(character.transform.position, searchNodeRadius, cellsMask);
            Collider[] startingCells = Physics.OverlapSphere(character.transform.position, searchNodeRadius, cellsMask);
            if (startingCells.Length == 0)
                return;

            gridSearcher.start = this.GetNearestCell(startingCells, character.transform.position);
            gridSearcher.start.SetColor(gridSearcher.startColor);


            Collider[] endingCells = Physics.OverlapSphere(hit.point, searchNodeRadius, cellsMask);
            if (endingCells.Length == 0)
                return;

            gridSearcher.end = this.GetNearestCell(endingCells, hit.point);
            gridSearcher.end.SetColor(gridSearcher.endColor);

            gridSearcher.ClearAll();
            if (gridSearcher.end && gridSearcher.start)
                gridSearcher.Search();

            currentIndex = 0;
            currentCell = gridSearcher.path[currentIndex];
            character.SetPoint(currentCell.transform.position);
        }
    }

    private Cell GetNearestCell(Collider[] colliders, Vector3 point)
    {
        float minDist = 999999;
        Cell cell = new Cell();
        foreach (var item in colliders)
        {
            if (Vector3.Distance(item.transform.position, point) < minDist)
            {
                try
                {
                    if (item.GetComponent<Cell>() == null)
                        continue;

                    if (item.gameObject.GetComponent<Cell>().Transitable)
                    {
                        cell = item.gameObject.GetComponent<Cell>();
                        minDist = Vector3.Distance(item.transform.position, point);
                    }
                }
                catch (Exception)
                {
                    print("Trying to take Cell component on a non cell object.");
                }
            }
        }
        return cell;
    }
}
