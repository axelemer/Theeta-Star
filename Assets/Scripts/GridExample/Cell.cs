using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    //Evento al seleccionar
    public event Action<Cell> OnSelect = delegate { };

    public Vector2Int pos;//Posicion en la grilla

    public Color defaultColor = Color.white;
    public Color untransitableColor = Color.black;

    private bool transitable = true;
    private int cost = 1;

    private MeshRenderer rend;
    private Text costText;
    private Collider2D coll;

    private int transitableLayer = 8;
    private int blockedLayer = 9;

    public bool Transitable
    {
        get
        {
            return transitable;
        }

        set
        {
            SetColor(value ? defaultColor : untransitableColor);
            costText.gameObject.SetActive(value);
            gameObject.layer = value ? transitableLayer : blockedLayer;
            transitable = value;
        }
    }

    public int Cost
    {
        get
        {
            return cost;
        }

        set
        {
            cost = value;
            costText.text = cost.ToString();
        }
    }

    private void Awake()
    {
        rend = GetComponentInChildren<MeshRenderer>();
        costText = GetComponentInChildren<Text>();
        coll = GetComponent<Collider2D>();
        Reset();
    }

    public void Reset()
    {
        Cost = 1;
        Transitable = true;
        SetColor(defaultColor);
    }

    public void SetColor(Color color)
    {
        rend.material.color = color;
    }

    public void ShowText(bool on)
    {
        costText.enabled = on;
    }

    public void Selected()
    {
        OnSelect(this);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0)) OnSelect(this);
        if (Input.GetMouseButtonUp(1)) Transitable = !Transitable;

        if (Input.GetKey(KeyCode.LeftShift)) Cost = Cost + (int)Input.mouseScrollDelta.y * 100;
        else Cost = Cost + (int)Input.mouseScrollDelta.y;

        Cost = Mathf.Clamp(Cost, 1, 100);
    }

}
