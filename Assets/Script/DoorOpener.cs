using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class DoorOpener : MonoBehaviour
{
    [Header("Level target")]
    [SerializeField] string nextSceneName = "Level2";

    [Header("Règles")]
    [SerializeField] bool requireAllGems = true; // L2 = true, L1 = false

    [Header("FX")]
    [SerializeField] float exitDelay = 2f;
    [SerializeField] bool fadePlayerOut = true;
    [SerializeField] VictoryUI victoryUI;   // <-- à relier dans l’Inspector

    bool isOpen = false;

    void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        if (requireAllGems && GemGoalManager.Instance)
            GemGoalManager.Instance.OnCountChanged += (_, __) => Recompute();
    }

    void Start() => Recompute();

    void Recompute()
    {
        if (!requireAllGems) isOpen = true;
        else isOpen = GemGoalManager.Instance &&
                      GemGoalManager.Instance.Current >= GemGoalManager.Instance.requiredGems;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !isOpen) return;
        StartCoroutine(WinSequence(other.gameObject));
    }

    IEnumerator WinSequence(GameObject player)
    {
        // coupe les scripts joueur
        foreach (var c in player.GetComponents<MonoBehaviour>())
            if (c.enabled && c.GetType().Name.StartsWith("Player")) c.enabled = false;
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb) { rb.linearVelocity = Vector2.zero; rb.bodyType = true; }

        // UI victoire
        if (victoryUI) victoryUI.PlayVictory();

        // petit fade sur le sprite du joueur
        if (fadePlayerOut)
        {
            var sr = player.GetComponentInChildren<SpriteRenderer>();
            if (sr)
            {
                float t = 0f; var col = sr.color;
                while (t < exitDelay) { t += Time.deltaTime; col.a = 1f - t/exitDelay; sr.color = col; yield return null; }
                col.a = 0f; sr.color = col;
            }
            else yield return new WaitForSeconds(exitDelay);
        }
        else yield return new WaitForSeconds(exitDelay);

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}
