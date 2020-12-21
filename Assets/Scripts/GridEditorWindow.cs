using UnityEngine;
using UnityEditor;

public class GridEditorWindow : EditorWindow
{
    [MenuItem("Window/Grid Creator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<GridEditorWindow>("Grid Creator");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Create"))
        {
            var gridMaker = FindObjectOfType<GridMaker>();

            if (gridMaker != null)
            {
                if (gridMaker.cells == null)
                {
                    //gridMaker.CreateGrid();
                    //gridMaker.gridCreated = true;
                }
            }
            else
            {
                Debug.Log("Grid maker not found.");
            }
        }
        if (GUILayout.Button("Delete"))
        {
            var gridMaker = FindObjectOfType<GridMaker>();

            if (gridMaker != null)
            {
                //if (gridMaker.cells != null)
                //    gridMaker.DestroyGrid();
            }
            else
            {
                Debug.Log("Grid maker not found.");
            }
        }
    }
}
