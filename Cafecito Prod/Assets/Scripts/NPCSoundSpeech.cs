using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NPCSoundSpeech : MonoBehaviour
{
    /// <summary>
    /// Where the audio comes from.
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// On mouse entering, play sound clip.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnTalk(AudioClip speechClip)
    {
        if (audioSource != null && speechClip != null)
        {
            audioSource.PlayOneShot(speechClip);
            Debug.Log("Sound played");
        }
        else
        {
            Debug.LogError("No audio source or clip found");
        }
    }
}
