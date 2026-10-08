using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public event Action OnPause;
    public event Action OnUnpause;
    public event Action OnGameEnd;

    [SerializeField] private GameObject pauseMenu;

    /// <summary>
    /// Opens or closes the pause menu.
    /// </summary>
    public void Resume()
    {
        // Reset the first button selection so hover works.
        EventSystem.current.SetSelectedGameObject(null);
        if (this.pauseMenu.gameObject.activeInHierarchy)
        {
            this.pauseMenu.SetActive(false);
            this.OnUnpause?.Invoke();
        }
        else
        {
            this.pauseMenu.SetActive(true);
            this.OnPause?.Invoke();
        }
    }

    public void Settings()
    {
        var sceneManager = FindFirstObjectByType<SceneManager>();
        sceneManager.SettingScene();
        this.pauseMenu.SetActive(false);
    }

    public void Menu()
    {
        var npcManager = FindFirstObjectByType<NPCManager>();
        npcManager.GameEnd();
        this.pauseMenu.SetActive(false);
        this.OnGameEnd?.Invoke();
    }

    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
