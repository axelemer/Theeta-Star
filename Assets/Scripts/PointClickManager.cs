using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointClickManager : MonoBehaviour
{
    public CharacterMovement characterMovement;


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
    }

    void Clicked()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {
            this.characterMovement.SetPoint(hit.point);
        }
    }
}
