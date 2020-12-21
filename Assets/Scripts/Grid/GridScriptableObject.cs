using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GridScriptableObject", order = 1)]
public class GridScriptableObject : ScriptableObject
{
    [SerializeField]
    public Cell[,] cells;
    internal bool gridCreated;
}
