using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private PauseMenu pauseMenu;

    private void Update()
    {
        // On left click.
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                NPC npc = hit.collider.GetComponent<NPC>();
                if (npc != null)
                {
                    npc.Talk();
                }
                else
                {
                    Debug.Log("No npc collider hit");
                }
            }
        }

        // On escape key clicked.
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            pauseMenu.Resume();
        }
    }
}
