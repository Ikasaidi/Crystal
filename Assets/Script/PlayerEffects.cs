using UnityEngine;
using System.Collections;

public class PlayerEffects : MonoBehaviour
{
    [Header("Contrôles")]
    public int inputSign = 1;                // 1 normal, -1 inversé

    [Header("Assombrissement monde (le joueur reste visible)")]
    [Range(0f, 1f)] public float darkAlpha = 0.7f; // opacité du noir
    public float fadeTime = 0.12f;                 // vitesse fondu
    public float margin = 2f; // marge autour de l'écran pour éviter les bords

    // internes
    Camera cam;
    SpriteRenderer playerSR;
    GameObject darkGO;
    SpriteRenderer darkSR;
    bool darknessActive = false;
    Coroutine running;

    void Awake()
    {
        cam = Camera.main;
        playerSR = GetComponent<SpriteRenderer>();
        EnsureWorldOverlay();         // crée le “WorldDarkness (Auto)” si besoin
        SetAlpha(0f);                 // invisible au départ
        darkGO.SetActive(false);
    }

    public void ApplyCursed(float duration)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(CursedRoutine(duration));
    }

    IEnumerator CursedRoutine(float duration)
    {
        inputSign = -1;
        TurnOn();

        yield return StartCoroutine(FadeTo(darkAlpha, fadeTime));
        yield return new WaitForSeconds(duration);

        inputSign = 1;
        yield return StartCoroutine(FadeTo(0f, fadeTime));
        TurnOff();

        running = null;
    }

    // 1) Création de l’overlay (sprite 1x1 unité, pas minuscule)
void EnsureWorldOverlay()
{
    if (darkSR != null) return;

    darkGO = new GameObject("WorldDarkness (Auto)");
    darkSR = darkGO.AddComponent<SpriteRenderer>();

    // sprite blanc 1x1 unité (PPU = 1) pour qu'on puisse le scaler facilement
    var tex = Texture2D.whiteTexture;
    var spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1f);
    darkSR.sprite = spr;

    darkSR.color = new Color(0f, 0f, 0f, 0f);
    if (playerSR)
    {
        // l’overlay est sur la même Sorting Layer que le joueur,
        // et juste en dessous dans l’ordre d’affichage
        darkSR.sortingLayerID = playerSR.sortingLayerID;
        darkSR.sortingOrder   = playerSR.sortingOrder - 1;
    }
}

// 2) Mise à l’échelle chaque frame pour couvrir toute la vue caméra
void LateUpdate()
{
    if (!darknessActive || darkSR == null) return;
    if (!cam) cam = Camera.main;
    if (!cam) return;

    // taille monde de ce que voit la caméra orthographique
    float height = cam.orthographicSize * 2f + margin;
    float width  = height * cam.aspect + margin;

    // position au centre de l’écran et scale pour tout couvrir
    darkGO.transform.position   = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
    darkGO.transform.localScale = new Vector3(width, height, 1f);

    // s'assurer que l’overlay reste juste sous le joueur
    if (playerSR)
    {
        darkSR.sortingLayerID = playerSR.sortingLayerID;
        darkSR.sortingOrder   = playerSR.sortingOrder - 1;
    }
}

    void TurnOn()
    {
        darknessActive = true;
        darkGO.SetActive(true);
        // place juste sous le player
        if (playerSR) { darkSR.sortingLayerID = playerSR.sortingLayerID; darkSR.sortingOrder = playerSR.sortingOrder - 1; }
    }

    void TurnOff()
    {
        darknessActive = false;
        darkGO.SetActive(false);
    }

    IEnumerator FadeTo(float targetA, float t)
    {
        float startA = darkSR.color.a;
        float e = 0f;
        while (e < t)
        {
            e += Time.deltaTime;
            float a = Mathf.Lerp(startA, targetA, t > 0f ? e / t : 1f);
            SetAlpha(a);
            yield return null;
        }
        SetAlpha(targetA);
    }

    void SetAlpha(float a)
    {
        if (!darkSR) return;
        var c = darkSR.color;
        c.a = Mathf.Clamp01(a);
        darkSR.color = c;
    }
}
