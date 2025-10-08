using UnityEngine;
using System.Collections;

public class VictoryUI : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup backgroundDim;   // image plein écran (noir alpha)
    public CanvasGroup victoryPanel;    // panneau centré (CanvasGroup)

    [Header("Effets")]
    public float fadeDuration = 0.6f;   // vitesse du fade
    public AudioSource victoryMusic;    // optionnel

    [Header("Anim optionnelle")]
    public Animator victoryAnimator;    // optionnel (trigger "Play")
    public string victoryTrigger = "Play";

    void Awake()
    {
        if (backgroundDim) { backgroundDim.alpha = 0; backgroundDim.interactable = false; backgroundDim.blocksRaycasts = false; }
        if (victoryPanel)  { victoryPanel.alpha  = 0; victoryPanel.interactable  = false; victoryPanel.blocksRaycasts  = false; }
    }

    public void PlayVictory()
    {
        if (victoryAnimator && !string.IsNullOrEmpty(victoryTrigger)) victoryAnimator.SetTrigger(victoryTrigger);
        if (victoryMusic) victoryMusic.Play();
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        if (backgroundDim) { backgroundDim.interactable = true; backgroundDim.blocksRaycasts = true; }
        if (victoryPanel)  { victoryPanel.interactable  = true; victoryPanel.blocksRaycasts  = true; }

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / fadeDuration);
            if (backgroundDim) backgroundDim.alpha = a * 0.85f;  // fond sombre
            if (victoryPanel)  victoryPanel.alpha  = a;          // panneau
            yield return null;
        }

        if (backgroundDim) backgroundDim.alpha = 0.85f;
        if (victoryPanel)  victoryPanel.alpha  = 1f;
    }
}
