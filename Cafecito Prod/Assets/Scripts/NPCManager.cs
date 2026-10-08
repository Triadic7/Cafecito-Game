using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public event Action<NPC> OnNpcReachedCounter;
    public event Action OnNpcReachedExit;
    public event Action OnNpcFinishedTalking;
    public event Action OnLastNPCExit;
    public event Action OnGameEnd;

    /// <summary>
    /// The npc thats currently active and heading to the counter.
    /// </summary>
    public NPC ActiveNPC {  get; private set; }

    public NPC IncomingNPC {  get; private set; }

    [SerializeField]
    private List<GameObject> npcs = new List<GameObject>();

    [SerializeField]
    private List<GameObject> npcsAlreadySpawned = new List<GameObject>();

    private List<GameObject> npcsDespawned = new List<GameObject>();

    [SerializeField] private Path[] paths;

    /// <summary>
    /// The min time for an npc to spawn.
    /// </summary>
    [SerializeField] private float minSpawnTime = 0.1f;

    /// <summary>
    /// The max time for an npc to spawn.
    /// </summary>
    [SerializeField] private float maxSpawnTime = 3f;

    public bool SpawnedAllNpcs 
    { 
        get 
        {
            return npcsDespawned.Count >= npcs.Count; 
        } 
    }

    /// <summary>
    /// On start.
    /// </summary>
    private void Start()
    {
        FindObjectOfType<SceneManager>().OnGameStart += () => 
        {
            StartGame();
            Debug.Log("Game Started");
        };
    }

    /// <summary>
    /// Spawns an npc and sets their path, then subscribes to events.
    /// </summary>
    private void SpawnNpc()
    {
        if (!SpawnedAllNpcs)
        {
            List<GameObject> npcList = this.npcs.Where(n => !npcsAlreadySpawned.Contains(n)).ToList();
            if (npcList.Count > 0) 
            {
                GameObject npc = npcList[UnityEngine.Random.Range(0, npcList.Count)];
                this.npcsAlreadySpawned.Add(npc.gameObject);

                Debug.Log($"Spawning npc {npc.name}");

                NPC npcComponent = npc.GetComponent<NPC>();

                if (ActiveNPC == null)
                {
                    ActiveNPC = npcComponent;
                }
                else
                {
                    IncomingNPC = npcComponent;
                }

                SetNpcPath(npcComponent);

                SubscribeToNpcEvents(npcComponent);
            }
        }
    }

    /// <summary>
    /// Sets a path for the npc to follow.
    /// </summary>
    /// <param name="npc">The npc.</param>
    private void SetNpcPath(NPC npc)
    {
        Path fullPath = null;

        // Choose path based on npc preference.
        if (npc.StartingPosition == NPC.StartSpawnPosition.Left)
        {
            fullPath = paths[0];
        }
        else if (npc.StartingPosition == NPC.StartSpawnPosition.Right)
        {
            fullPath = paths[1];
        }
        else if (npc.StartingPosition == NPC.StartSpawnPosition.Random)
        {
            int leftOrRight = UnityEngine.Random.Range(0, 2) == 0 ? 0 : 1;
            Debug.Log($"Choosing {leftOrRight} path on NPC {npc.Name}");
            fullPath = paths[leftOrRight];
        }

        // Move NPC instantly to the chosen start.
        npc.transform.position = fullPath.PathNodes[0].transform.position;

        npc.SetPathing(fullPath);
    }

    /// <summary>
    /// Despawns an npc.
    /// </summary>
    /// <param name="npc">The npc to despawn.</param>
    private void DespawnNpc(NPC npc)
    {
        this.OnNpcReachedExit?.Invoke();
        this.npcsDespawned.Add(npc.gameObject);

        UnsubscribeToNpcEvents(npc);

        // If all npcs have went.
        if(SpawnedAllNpcs)
        {
            Debug.Log("Last npc spawned");
            this.OnLastNPCExit?.Invoke();
        }
        else
        {
            Debug.Log("Not all npcs gone.");
        }
    }

    /// <summary>
    /// Subscribes to npc events on npc.
    /// </summary>
    /// <param name="npc">The npc to subscribe.</param>
    private void SubscribeToNpcEvents(NPC npc)
    {
        OnNpcFinishedTalking += npc.FinishConversation;
        npc.OnReachedCounter += OnNpcReachedCounter;
        npc.OnReachedExit += DespawnNpc;
        npc.OnReachedNearExit += HandleActiveNPCSwap;

        // On pause.
        var pauseMenu = FindFirstObjectByType<PauseMenu>();
        pauseMenu.OnPause += npc.OnPause;
        pauseMenu.OnUnpause += npc.OnUnpause;
    }

    /// <summary>
    /// Unsubscribes to npc events on npc.
    /// </summary>
    /// <param name="npc">The npc to unsubscribe.</param>
    private void UnsubscribeToNpcEvents(NPC npc)
    {
        OnNpcFinishedTalking -= npc.FinishConversation;
        npc.OnReachedExit -= DespawnNpc;
        npc.OnReachedNearExit -= HandleActiveNPCSwap;

        var pauseMenu = FindFirstObjectByType<PauseMenu>();
        pauseMenu.OnPause -= npc.OnPause;
        pauseMenu.OnUnpause -= npc.OnUnpause;
    }

    /// <summary>
    /// Makes the incoming npc the active npc.
    /// </summary>
    private void HandleActiveNPCSwap()
    {
        SpawnNpcWithDelay();
        ActiveNPC = IncomingNPC;
    }

    /// <summary>
    /// Starts the game by spawning in npcs.
    /// </summary>
    private void StartGame()
    {
        SpawnNpcWithDelay();
    }

    /// <summary>
    /// Spawns an npc with a delay.
    /// </summary>
    private void SpawnNpcWithDelay()
    {
        float time = UnityEngine.Random.Range(minSpawnTime, maxSpawnTime);
        StartCoroutine(SpawnNpcAfterTime(time));
    }

    /// <summary>
    /// Spawns an npc after some time.
    /// </summary>
    /// <param name="time">The time until it spawns an npc.</param>
    /// <returns></returns>
    private IEnumerator SpawnNpcAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        SpawnNpc();
    }

    /// <summary>
    /// Used by the give coffee button to make the npc resume pathing.
    /// </summary>
    public void FinishConversation()
    {
        OnNpcFinishedTalking?.Invoke();
        if (ActiveNPC != null)
        {
            ActiveNPC.ResumePathing();
        }
    }

    /// <summary>
    /// Ends the game.
    /// </summary>
    public void GameEnd()
    {
        OnGameEnd?.Invoke();
        foreach(var npc in npcsAlreadySpawned)
        {
            var npcComponent = npc.GetComponent<NPC>();
            npcComponent.OnGameEnd();
            UnsubscribeToNpcEvents(npcComponent);
        }
        this.npcsAlreadySpawned.Clear();
        this.npcsDespawned.Clear();
        this.ActiveNPC = null;
        Debug.Log($"Game end");
    }

}
