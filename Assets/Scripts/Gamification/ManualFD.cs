using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ManualFD : MonoBehaviour
{
    public Text noteText;
    public GameObject notebookPanel;
    private Queue<string> notes = new Queue<string>();

    public void ShowNote(string note)
    {
        if (notebookPanel != null)
        {
            notebookPanel.SetActive(true);
        }

        notes.Enqueue(note);
        DisplayNextNote();
    }

    private void DisplayNextNote()
    {
        if (notes.Count > 0 && noteText != null)
        {
            noteText.text = notes.Dequeue();
        }
    }

    public void OnNextNoteButton()
    {
        DisplayNextNote();
    }
}
