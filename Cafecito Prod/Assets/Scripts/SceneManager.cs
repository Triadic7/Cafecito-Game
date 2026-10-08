using Assets.Scripts;
using System;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public event Action OnGameStart;

    public enum Scenes {MainMenu, Shop, Talking, Bibliograpghy };
    public Scenes CurrentScene;

    [SerializeField] private Transform startSceneTransform;
    [SerializeField] private Transform coffeeSceneTransform;
    [SerializeField] private Transform talkingSceneTransform;
    [SerializeField] private Transform behindCounterTransform;
    [SerializeField] private Transform bibliographyTransform;
    [SerializeField] private Transform settingsTransform;
    [SerializeField] private GameObject startPanel;

    [SerializeField] private Camera camera;

    private CameraFader fader;

    private void Start()
    {
        camera.transform.position = startSceneTransform.position;
        fader = FindObjectOfType<CameraFader>();

        NPCManager npcManager = FindObjectOfType<NPCManager>();
        npcManager.OnNpcReachedCounter += (npc) =>
        {
            fader.FadeToCamera(
                talkingSceneTransform,
                onFadeInComplete: null,
                onFadeOut: () =>
                {
                    // Move NPC behind counter while black.
                    npc.transform.position = behindCounterTransform.position;
                }
            );
            CurrentScene = Scenes.Talking;
        };

        npcManager.OnNpcFinishedTalking += () =>
        {
            fader.FadeToCamera(
                coffeeSceneTransform,
                onFadeInComplete: null,
                onFadeOut: () =>
                {
                    npcManager.ActiveNPC.IsTalking = false;
                    // Move NPC to the end of their path while black.
                    npcManager.ActiveNPC.transform.position = npcManager.ActiveNPC.PathingPoints.PathNodes[npcManager.ActiveNPC.PathingPoints.PathNodes.Count - 1].transform.position;
                }
            );
            CurrentScene = Scenes.Shop;
        };
        npcManager.OnGameEnd += () => StartSceneChange(startSceneTransform);
    }

    public void StartGame()
    {
        OpenStartGamePanel(false);
        CurrentScene = Scenes.Shop;
        StartSceneChange(coffeeSceneTransform, () => OnGameStart?.Invoke());
    }

    public void BibliographyScene()
    {
        CurrentScene = Scenes.Bibliograpghy;
        StartSceneChange(bibliographyTransform);
    }

    public void SwitchToCurrentScene()
    {
        switch (CurrentScene)
        {
            case Scenes.MainMenu:
                StartScene();
                break;

            case Scenes.Shop:
                ShopScene();
                break;

            case Scenes.Talking:
                TalkingScene();
                break;

            case Scenes.Bibliograpghy:
                BibliographyScene();
                break;
        }
    }

    public void StartScene()
    {
        CurrentScene = Scenes.MainMenu;
        StartSceneChange(startSceneTransform);
    }

    public void ShopScene()
    {
        CurrentScene = Scenes.Shop;
        StartSceneChange(coffeeSceneTransform);
    }

    public void TalkingScene()
    {
        CurrentScene = Scenes.Talking;
        StartSceneChange(talkingSceneTransform);
    }

    public void BiblioScene()
    {
        CurrentScene = Scenes.Bibliograpghy;
        StartSceneChange(bibliographyTransform);
    }

    public void SettingScene()
    {
        StartSceneChange(settingsTransform);
    }

    public void OpenStartGamePanel(bool open)
    {
        startPanel.SetActive(open);
    }

    private void StartSceneChange(Transform targetTransform, Action onComplete = null)
    {
        if (fader != null)
        {
            fader.FadeToCamera(targetTransform, onFadeInComplete: onComplete);
        }
        else
        {
            camera.transform.position = targetTransform.position;
            onComplete?.Invoke();
        }
    }
}
