using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class CameraFader : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (fadeImage != null)
        {
            fadeImage.color = Color.clear;
        }
    }

    // Fades out, moves the camera, then fades back in
    public void FadeToCamera(Transform cameraTransform, Action onFadeInComplete = null, Action onFadeOut = null)
    {
        StartCoroutine(FadeRoutine(cameraTransform, onFadeInComplete, onFadeOut));
    }

    // Fades out, executes a callback while black, then fades back in
    public void FadeOutThen(Action onBlackComplete)
    {
        StartCoroutine(FadeRoutine(null, null, onBlackComplete));
    }

    private IEnumerator FadeRoutine(Transform cameraTransform, Action onFadeInComplete, Action onFadeOut)
    {
        // Fade to black
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = Color.black;

        // Call the fadeOut callback
        onFadeOut?.Invoke();

        // Move the camera if provided
        if (cameraTransform != null)
            Camera.main.transform.position = cameraTransform.position;

        // Call the fadeIn callback
        onFadeInComplete?.Invoke();

        // Fade back in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = Color.clear;
    }
}