using UnityEngine;
using System.Collections;

public class FadeOut2D : MonoBehaviour
{
    [Header("Fade")]
    public float fadeDuration = 1f; // durée du fondu
    public bool keepInvisibleAfter = true; // rester invisible à la fin (en général oui si on change de scène)

    private SpriteRenderer sr;
    private bool fading = false;
    private float t = 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (!sr) sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (!fading || sr == null) return;

        t += Time.deltaTime;
        float alpha = Mathf.Clamp01(1f - (t / fadeDuration));
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

        if (t >= fadeDuration)
        {
            fading = false;
            if (!keepInvisibleAfter)
            {
                // si tu veux re-montrer le joueur (pas utile si tu charges une scène)
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            }
        }
    }

    // <<< Appelle ça quand tu veux démarrer le fondu >>>
    public void BeginFade()
    {
        if (fading) return;
        t = 0f;
        fading = true;
    }
}
