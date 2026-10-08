using UnityEngine;

public class NPCClick : MonoBehaviour
{
    public void OnMouseDown()
    {
        Debug.Log("NPC clicked!");
        NPCClicked();
    }

    private void NPCClicked()
    {
        Debug.Log("Open dialogue with this NPC.");
    }
}
