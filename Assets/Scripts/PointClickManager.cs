using System;
using System.Collections;
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
    private Vector3 point;
    public float timeToSetPath;
    private float timerPath;

    void Update()
    {
        CheckClicks();
        timerPath += Time.deltaTime;
        if (timerPath >= timeToSetPath && character.onMove)
        {
            if (gridSearcher.path[currentIndex] != null && gridSearcher.path[currentIndex++] != null && Physics.Raycast(character.transform.position, 
                                                                                                                        gridSearcher.path[currentIndex].transform.position, 
                                                                                                                        Vector3.Distance(character.transform.position, 
                                                                                                                        gridSearcher.path[currentIndex].transform.position), 
                                                                                                                        gridSearcher.blockedMask))
            {
                CalculatePath();
            }
        }
    }

    private void CalculatePath()
    {
        timerPath = 0f;

        bool checkCells = Physics.CheckSphere(character.transform.position, searchNodeRadius, cellsMask);
        Collider[] startingCells = Physics.OverlapSphere(character.transform.position, searchNodeRadius, cellsMask);
        if (startingCells.Length == 0)
            return;

        gridSearcher.start = this.GetNearestCell(startingCells, character.transform.position);
        gridSearcher.start.SetColor(gridSearcher.startColor);

        if (gridSearcher.start == null)
            return;

        Collider[] endingCells = Physics.OverlapSphere(point, searchNodeRadius, cellsMask);
        if (endingCells.Length == 0)
            return;

        gridSearcher.end = this.GetNearestCell(endingCells, point);
        gridSearcher.end.SetColor(gridSearcher.endColor);

        if (gridSearcher.end == null)
            return;

        gridSearcher.ClearAll();
        if (gridSearcher.end && gridSearcher.start)
            gridSearcher.Search();

        if (gridSearcher.path[1] != null && gridSearcher.start != null && Physics.Raycast(character.transform.position, gridSearcher.start.transform.position, Mathf.Infinity, gridSearcher.blockedMask))
        {
            currentIndex = 1;
        }
        else
        {
            currentIndex = 0;
        }
        currentCell = gridSearcher.path[currentIndex];
        character.SetPoint(currentCell.transform.position);

    }

    private void CheckClicks()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Clicked();
        }

        if (gridSearcher.path.Count > 0)
        {
            SetPathToCharacter();
        }
    }

    private void SetPathToCharacter()
    {
        if (Vector3.Distance(character.transform.position, currentCell.transform.position) < distanceToNextNode)
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
            this.point = hit.point;
            CalculatePath();
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
                    if (item.GetComponent<Cell>() != null)
                    {
                        Cell tempCell = item.GetComponent<Cell>();

                        if (tempCell.Transitable)
                        {
                            cell = tempCell;
                            minDist = Vector3.Distance(item.transform.position, point);
                        }
                    }
                }
                catch (Exception)
                {
                    print("Trying to take Cell component on a non cell object." + item.gameObject.name);
                }
            }
        }
        return cell;
    }

}
