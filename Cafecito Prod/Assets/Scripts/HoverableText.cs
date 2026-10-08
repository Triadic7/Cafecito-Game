using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HoverableText : Hoverable
{
    [SerializeField] private string textToDisplay;

    private void Awake()
    {
        this.OnHoverObject += DisplayText;
    }

    private void DisplayText(GameObject gameObject)
    {
        gameObject.GetComponentInChildren<TMP_Text>().text = textToDisplay;
    }
}
