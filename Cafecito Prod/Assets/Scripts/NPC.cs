using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    public class NPC : MonoBehaviour
    {
        public event Action<NPC> OnReachedCounter;
        public event Action<PathNode> OnReachedPathNode;

        /// <summary>
        /// On npc reaching 3/4 to exit on way back.
        /// </summary>
        public event Action OnReachedNearExit;
        public event Action<NPC> OnReachedExit;

        [SerializeField]
        public string[] DialogueOptions;

        [SerializeField]
        public string[] AnswerOptions;

        [SerializeField]
        public string[] SourceBlurbs;

        public AudioClip[] AnswerVoiceClips;

        public Path PathingPoints;

        public bool IsTalking;

        public bool StopsToLookInWindow = false;

        public enum StartSpawnPosition {Left, Right, Random};
        public StartSpawnPosition StartingPosition = StartSpawnPosition.Random;

        public string Name;

        private AudioSource walkSound;

        private int currentTargetIndex;

        private CapsuleCollider2D clickCollider;

        /// <summary>
        ///  1 = forward, -1 = backward
        /// </summary>
        private int direction = 1;

        [SerializeField]
        private float moveSpeed = 10f;
        private float currentSpeed;

        private bool isActive;

        private bool nearedExit;

        private bool isReadyToTalk;

        private Coroutine pathingRoutine;

        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private GameObject exclamationMark;

        private Animator animator;

        private void Awake()
        {
            clickCollider = GetComponentInChildren<CapsuleCollider2D>();
            clickCollider.enabled = false;
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogWarning("NPC: No SpriteRenderer found on this GameObject!");
            }

            animator = GetComponentInChildren<Animator>();
            walkSound = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Npc begins talk at the counter.
        /// </summary>
        public void Talk()
        {
            ReachedCounter();
        }

        /// <summary>
        /// Sets the npc pathing.
        /// </summary>
        /// <param name="pathing">The path the npc will take.</param>
        public void SetPathing(Path pathing)
        {
            this.exclamationMark.SetActive(false);
            this.isActive = true;
            this.nearedExit = false;
            this.isReadyToTalk = false;
            this.PathingPoints = pathing;
            this.currentTargetIndex = 0;
            this.direction = 1;
            this.animator.speed = 1;
            
            this.currentSpeed = moveSpeed;
            this.IsTalking = false;
            this.walkSound.volume = 0;

            this.OnReachedPathNode += StopAtWindow;
            this.OnReachedPathNode += SwapSorting;

            // Start moving.
            if (pathingRoutine != null)
            {
                StopCoroutine(pathingRoutine);
            }

            pathingRoutine = StartCoroutine(StartPathing());
        }

        public void OnPause()
        {
            this.currentSpeed = 0;
            this.StopWalkSound();
            this.animator.speed = 0;
        }

        public void OnUnpause()
        {
            if (!this.isReadyToTalk)
            {
                this.currentSpeed = this.moveSpeed;
                this.StartWalkSound();
                this.animator.speed = 1;
            }
        }

        public void OnGameEnd()
        {
            ResetNpc();
        }

        private void ResetNpc()
        {
            this.exclamationMark.SetActive(false);
            this.currentSpeed = 0;
            this.isActive = false;
            this.DecreaseSortingOrder();
            this.StopWalkSound();
            this.gameObject.transform.position = this.PathingPoints.PathNodes[0].transform.position;
            this.currentTargetIndex = 0;
            this.direction = 1;
            this.PathingPoints = null;
        }

        /// <summary>
        /// Starts pathing the npc on their pathing route.
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartPathing()
        {
            StartWalkSound();
            Debug.Log($"NPC Moving");
            while (PathingPoints != null && PathingPoints.PathNodes.Count > 0 && isActive)
            {
                if (!IsTalking)
                {
                    Transform target = PathingPoints.PathNodes[currentTargetIndex].gameObject.transform;

                    // Move until close enough to the target.
                    while (Vector2.Distance(transform.position, target.position) > 0.05f)
                    {
                        transform.position = Vector2.MoveTowards(this.transform.position, target.position, this.currentSpeed * Time.deltaTime);
                        yield return null;
                    }

                    // Trigger event if reached the counter.
                    if (this.currentTargetIndex == this.PathingPoints.PathNodes.Count - 1 && this.direction == 1)
                    {
                        WaitForClickToTalkToPlayer();
                    }

                    // Trigger event if reached the exit.
                    if (this.currentTargetIndex == 0 && this.direction == -1)
                    {
                        ReachedExit();
                    }

                    // Advance index.
                    OnReachedNode();

                    // Reverse if reached ends.
                    if (this.currentTargetIndex >= this.PathingPoints.PathNodes.Count)
                    {
                        // Reverse.
                        this.currentTargetIndex = this.PathingPoints.PathNodes.Count - 2;
                        this.direction = -1;
                    }
                    else if (currentTargetIndex < 0)
                    {
                        // Forward.
                        this.currentTargetIndex = 1;
                        this.direction = 1;
                    }
                }

                yield return null;
            }
        }

        /// <summary>
        /// Increases the sorting order on the npc.
        /// </summary>
        private void IncreaseSortingOdrder()
        {
            spriteRenderer.sortingOrder = 4;
            Debug.Log("NPC: sorting order increased (forward halfway).");
        }

        /// <summary>
        /// Descreases the sorting on the npc.
        /// </summary>
        private void DecreaseSortingOrder()
        {
            spriteRenderer.sortingOrder = 0;
            Debug.Log("NPC: sorting order reset (backward halfway).");
        }

        /// <summary>
        /// Npc stops at the window.
        /// </summary>
        /// <param name="pathNode"></param>
        private void StopAtWindow(PathNode pathNode)
        {
            if (this.StopsToLookInWindow && this.direction == 1)
            {
                StartCoroutine(WatchInWindow(pathNode));
            }
        }

        /// <summary>
        /// Swaps the sorting layer depending on if the npc is entering or leaving.
        /// </summary>
        /// <param name="pathNode">The name of the path node.</param>
        private void SwapSorting(PathNode pathNode)
        {
            // If npc is at door, adjust sorting layer as needed.
            if (pathNode.PathNodeName == "Entrance")
            {
                Debug.Log("Entering entrance");

                // If entering, increase sorting layer.
                if (direction == 1)
                {
                    this.IncreaseSortingOdrder();
                }
                else
                {
                    // Else descrease it.
                    this.DecreaseSortingOrder();
                }

            }
        }

        /// <summary>
        /// Npc waits and watches in the window.
        /// </summary>
        /// <param name="pathNode"></param>
        /// <returns></returns>
        private IEnumerator WatchInWindow(PathNode pathNode)
        {
            if(pathNode.PathNodeName == "Window")
            {
                this.currentSpeed = 0;
                StopWalkSound();
                yield return new WaitForSeconds(3);
                this.currentSpeed = this.moveSpeed;
                animator.speed = 1f;
                StartWalkSound();
            }
        }

        /// <summary>
        /// On the conversation between player and npc done.
        /// </summary>
        public void FinishConversation()
        {
            // Stop movement and animation.
            IsTalking = true;
            currentSpeed = 0;
            animator.speed = 0f;

            spriteRenderer.sortingOrder = 4;
            
            Debug.Log("Ended talking");
        }

        /// <summary>
        /// Npc resumes moving on path.
        /// </summary>
        public void ResumePathing()
        {
            // Resume movement.
            IsTalking = false;
            isReadyToTalk = false;
            currentSpeed = moveSpeed;
            animator.speed = 1f;
            StartWalkSound();
        }

        /// <summary>
        ///  Allows the npc to be clicked on to open the dialogue menu.
        /// </summary>
        private void WaitForClickToTalkToPlayer()
        {
            this.isReadyToTalk = true;
            clickCollider.enabled = true;
            currentSpeed = 0;
            animator.speed = 0f;
            exclamationMark.SetActive(true);
            StopWalkSound();
        }

        /// <summary>
        ///  When npc is about to speak to player.
        /// </summary>
        private void ReachedCounter()
        {
            exclamationMark.SetActive(false);
            clickCollider.enabled = false;
            OnReachedCounter?.Invoke(this);

            // Pause NPC.
            IsTalking = true;
            currentSpeed = 0;
            animator.speed = 0f;

            spriteRenderer.sortingOrder = 4;
            Debug.Log("Reached counter");
        }

        /// <summary>
        /// When npc reaches the exit.
        /// </summary>
        private void ReachedExit()
        {
            this.currentSpeed = 0;
            this.isActive = false;
            this.isReadyToTalk = false;
            OnReachedExit?.Invoke(this);
            Debug.Log("Reached exit");
            StopWalkSound();
        }

        private void OnReachedNode()
        {
            OnReachedPathNode?.Invoke(this.PathingPoints.PathNodes[this.currentTargetIndex]);
            this.currentTargetIndex += this.direction;

            // If npc near the exit heading out.
            if(this.currentTargetIndex <= 2 && this.direction == -1 && !nearedExit)
            {
                nearedExit = true;
                OnReachedNearExit?.Invoke();
                Debug.Log("Npc getting near exit.");
            }

            // Change volume on walk sound based on distance.
            if(direction == 1)
            {
                walkSound.volume += 0.2f;
            }
            else if(direction == -1) 
            {
                walkSound.volume -= 0.2f;
            }
        }

        /// <summary>
        /// Plays the walk sound.
        /// </summary>
        private void StartWalkSound()
        {
            walkSound.Play();
        }

        /// <summary>
        /// Stops the walk sound.
        /// </summary>
        private void StopWalkSound()
        {
            walkSound.Stop();
        }
    }
}
