using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHoverSound : MonoBehaviour, IPointerEnterHandler
{
    /// <summary>
    /// Where the audio comes from.
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    ///  The sound of when hovering.
    /// </summary>
    public AudioClip hoverClip;

    /// <summary>
    /// On mouse entering, play sound clip.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioSource != null && hoverClip != null)
        {
            audioSource.PlayOneShot(hoverClip);
        }
        else
        {
            Debug.LogError("No audio source or clip found");
        }
    }
}
