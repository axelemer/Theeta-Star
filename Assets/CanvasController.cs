using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public GridMaker grid;
    private bool hideNodes = false;
    public void ButtonHideShowNodes()
    {
        DoHideShow();
    }

    public void DoHideShow()
    {
        foreach (var node in grid.cells)
        {
            if (hideNodes)
                node.SetColor(node.transaprent);
            if(!hideNodes)
                node.SetColor(node.defaultColor);

            hideNodes = !hideNodes;
        }
    }
}
