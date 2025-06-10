using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
public class CableConnector : MonoBehaviour
{

    private GameObject cable;
    private GameObject from;
    private GameObject to;
    private string statusText = "Waiting...";
    /*private void OnGUI()
    {
        from = EditorGUILayout.ObjectField("From", from, typeof(GameObject), true) as GameObject;
        to = EditorGUILayout.ObjectField("To", to, typeof(GameObject), true) as GameObject;
        bool enabled = (from != null && to != null);
        if (!enabled)
        {
            statusText = "Add a From and To object to proceed.";
        }
        UnityEngine.GUI.enabled = enabled;
        if (GUILayout.Button("Update Cable Connection"))
        {
            statusText = "== Connecting cable... ==";
            statusText += System.Environment.NewLine + "Done!";
        }
        // Draw status because yeh why not?
        statusContent.text = statusText;
        EditorStyles.label.wordWrap = true;
        GUILayout.Label(statusContent);
    }*/
}
