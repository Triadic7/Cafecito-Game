using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextHoverTranslator : MonoBehaviour
{
    [SerializeField] private TMP_Text[] textComponents;
    [SerializeField] private GameObject tooltipBox;
    [SerializeField] private GameObject sourceBox;
    [SerializeField] private TMP_Text tooltipText;

    private SceneManager.Scenes currentScene;

    // Dictionary for translations
    private Dictionary<string, string> translations = new Dictionary<string, string>()
    {
        { "mi_hija", "Literally my daughter, but is used generally as a term of endearment, similar to dear or little one" },
        { "jornaleros", "Day laborers"},
        { "esclavos", "Slaves" },
        { "la_isla", "The Island" },
        { "hacendados", "Plantation owners" },
        { "hacandado", "Plantation owner" },
        { "hacendado", "Plantation owner" },
        { "jíbaro", "Rural worker" },
        { "hacienda", "This could mean many different things, but in this context, the aprendiz is referring to the coffee plantation" },
        { "haciendas", "In this context, the coffee plantations" },
        { "si_claro", "Of course" },
        { "claro_que_si", "Of course" },
        { "asquerosos", "Disgusting. In this context, a disgusting or foul person" },
        { "si_y_no", "Yes and no" },
    };

    private void Awake()
    {
        tooltipBox.SetActive(false);
    }

    void Update()
    {
        bool foundHover = false;
        currentScene = FindFirstObjectByType<SceneManager>().CurrentScene;

        foreach (var textComponent in textComponents)
        {
            if (textComponent == null) continue;

            int linkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, Input.mousePosition, null);
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = textComponent.textInfo.linkInfo[linkIndex];
                string linkID = linkInfo.GetLinkID();

                if (translations.TryGetValue(linkID, out string translation))
                {
                    tooltipBox.SetActive(true);
                    sourceBox.SetActive(false);
                    tooltipText.text = translation;
                    foundHover = true;
                    break;
                }
            }
        }
        if (currentScene == SceneManager.Scenes.Talking)
        {
            if (!foundHover)
            {
                tooltipBox.SetActive(false); 
                sourceBox.SetActive(true);
            }
        }
    }
}
