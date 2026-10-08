using UnityEngine;
using System.Collections;

public class SkyGradientController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sky;
    [SerializeField] private Gradient topGradient;
    [SerializeField] private Gradient bottomGradient;

    private Material mat;

    private float[] segmentEndTimes = { 10f, 20f, 30, 40f, 60f };
    private int currentSegment = 0;

    private bool advanceSegment = false;
    private Coroutine segmentCoroutine;

    private CameraFader fader;

    void Start()
    {
        mat = sky.material;
        fader = FindObjectOfType<CameraFader>();
        SceneManager sceneManager = FindObjectOfType<SceneManager>();
        sceneManager.OnGameStart += StartSegments;
        NPCManager npcManager = FindObjectOfType<NPCManager>();
        npcManager.OnNpcReachedExit += NextSegment;
        npcManager.OnGameEnd += () =>
        {
            fader.FadeOutThen(() =>
            {
                ResetGradient();
            });
        };

        // Reset sky at the beginning but do not start coroutine.
        ResetGradient();
    }

    private void StartSegments()
    {
        // Stop any existing coroutine first
        if (segmentCoroutine != null)
        {
            StopCoroutine(segmentCoroutine);
        }

        currentSegment = 0;
        segmentCoroutine = StartCoroutine(RunSkySegments());
    }

    private IEnumerator RunSkySegments()
    {
        while (currentSegment < segmentEndTimes.Length)
        {
            float segmentStart = currentSegment == 0 ? 0 : segmentEndTimes[currentSegment - 1];
            float segmentEnd = segmentEndTimes[currentSegment];
            float segmentLength = segmentEnd - segmentStart;

            float elapsed = 0f;
            while (elapsed < segmentLength)
            {
                float t = Mathf.Clamp01(elapsed / segmentLength);
                float gradientTStart = (float)currentSegment / segmentEndTimes.Length;
                float gradientTEnd = (float)(currentSegment + 1) / segmentEndTimes.Length;
                Color top = topGradient.Evaluate(Mathf.Lerp(gradientTStart, gradientTEnd, t));
                Color bottom = bottomGradient.Evaluate(Mathf.Lerp(gradientTStart, gradientTEnd, t));

                mat.SetColor("_TopColor", top);
                mat.SetColor("_BottomColor", bottom);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Pause at the end of the segment until NextSegment is called.
            yield return new WaitUntil(() => advanceSegment);
            advanceSegment = false;

            currentSegment++;
        }
    }

    public void NextSegment()
    {
        advanceSegment = true;
    }

    public void ResetGradient()
    {
        if (segmentCoroutine != null)
        {
            StopCoroutine(segmentCoroutine);
        }

        currentSegment = 0;
        advanceSegment = false;

        Color top = topGradient.Evaluate(0f);
        Color bottom = bottomGradient.Evaluate(0f);

        mat.SetColor("_TopColor", top);
        mat.SetColor("_BottomColor", bottom);
    }

}
