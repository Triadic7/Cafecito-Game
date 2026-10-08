using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NPCTalkManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerTalkWindow;

    [SerializeField]
    private GameObject npcTalkWindow;

    [SerializeField]
    private GameObject endOfDayWindow;

    [SerializeField]
    private GameObject playerQuestionBox;

    [SerializeField]
    private TMP_Text npcNameBox;

    [SerializeField]
    private TMP_Text npcQuestionBox;

    [SerializeField]
    private TMP_Text npcAnswerBox;

    [SerializeField]
    private NPCSoundSpeech speech;

    [SerializeField]
    private GameObject coffeeButton;

    [SerializeField]
    private GameObject sourceBox;

    private NPC activeNpc;

    private int currentDialogueNode = 0;

    public void SelectOption(GameObject option)
    {
        if(this.activeNpc.AnswerVoiceClips.Length > 0 && this.activeNpc.AnswerVoiceClips.Length > currentDialogueNode)
        {
            if (this.activeNpc.AnswerVoiceClips[currentDialogueNode] != null)
            {
                speech.OnTalk(this.activeNpc.AnswerVoiceClips[currentDialogueNode]);
            }
            else
            {
                Debug.LogWarning($"No speech found at {currentDialogueNode} on NPC {this.activeNpc.Name}");
            }
        }
        else
        {
            Debug.Log($"No speech clips found on NPC {this.activeNpc.Name}");
        }

        // Linear dialogue.
        currentDialogueNode++;

        if (currentDialogueNode < activeNpc.DialogueOptions.Length) 
        {
            // Update player choice.
            UpdatePlayerOption(activeNpc.DialogueOptions[currentDialogueNode]);
            DisplayNPCWindow();
            ShowSourceOptions();

            // Reset the first button selection so hover works.
            EventSystem.current.SetSelectedGameObject(null);
        }

        if (currentDialogueNode == activeNpc.DialogueOptions.Length) 
        {
            DisplayNPCWindow();
            ShowCoffeeButton();
            this.playerQuestionBox.SetActive(false);
        }
    }

    private void Start()
    {
        this.playerTalkWindow.SetActive(false);
        this.npcTalkWindow.SetActive(false);

        NPCManager npcManager = FindObjectOfType<NPCManager>();
        npcManager.OnNpcReachedCounter += OpenDialogue;
        npcManager.OnNpcFinishedTalking += CloseDialogue;
        npcManager.OnLastNPCExit += DisplayEndScreen;
        npcManager.OnGameEnd += () => 
        {
            this.endOfDayWindow.SetActive(false);
            this.playerTalkWindow.SetActive(false);
            this.npcTalkWindow.SetActive(false);
            this.sourceBox.SetActive(false);
        };
    }

    private void ShowCoffeeButton()
    {
        this.coffeeButton.SetActive(true);
    }

    private void ShowSourceOptions()
    {
        this.sourceBox.SetActive(true);
        this.DisplaySourceBox();
    }

    private void HideCoffeeAndSourceOptions()
    {
        this.coffeeButton.SetActive(false);
        this.sourceBox.SetActive(false);
    }

    private void OpenDialogue(NPC nPC)
    {
        this.currentDialogueNode = 0;
        this.activeNpc = nPC;
        DisplayPlayerWindow(nPC);
        DisplayNPCWindow();
        ShowSourceOptions();
    }

    private void CloseDialogue()
    {
        this.playerTalkWindow.SetActive(false);
        this.npcTalkWindow.SetActive(false);
        this.HideCoffeeAndSourceOptions();
        this.activeNpc = null;
        this.currentDialogueNode = 0;
    }

    private void DisplayEndScreen()
    {
        this.endOfDayWindow.SetActive(true);
    }

    private void DisplayPlayerWindow(NPC npc)
    {
        this.playerTalkWindow.SetActive(true);
        this.playerQuestionBox.SetActive(true);

        // Reset the first button selection so hover works.
        EventSystem.current.SetSelectedGameObject(null);

        UpdatePlayerOption(activeNpc.DialogueOptions[currentDialogueNode]);
    }

    private void UpdatePlayerOption(string content)
    {
        this.playerTalkWindow.GetComponentInChildren<TMP_Text>().text = content;
    }

    /// <summary>
    /// Displays the content on the NPC side.
    /// </summary>
    /// <param name="content">What the NPC says.</param>
    private void DisplayNPCWindow()
    {
        this.npcTalkWindow.SetActive(true);
        this.npcNameBox.text = activeNpc.Name;

        // First dialogue option.
        if (this.currentDialogueNode > 0)
        {
            this.npcQuestionBox.text = $"You: {this.activeNpc.DialogueOptions[currentDialogueNode - 1]}";
            this.npcAnswerBox.text = $"{this.activeNpc.Name}: {this.activeNpc.AnswerOptions[currentDialogueNode]}";
        }
        else
        {
            this.npcQuestionBox.text = $"{this.activeNpc.Name}: {this.activeNpc.AnswerOptions[currentDialogueNode]}";
            this.npcAnswerBox.text = string.Empty;
        }
    }

    public void DisplaySourceBox()
    {
        Debug.Log("Displaying source box");
        this.sourceBox.GetComponentInChildren<TMP_Text>().text = this.activeNpc.SourceBlurbs[currentDialogueNode];
    }
}
