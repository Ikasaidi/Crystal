using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance { get; private set; }

    [Header("UI Vies (glisse tes 3 Images ici)")]
    public List<Image> heartImages = new List<Image>(); // ordre gauche→droite

    [Header("Options")]
    public string gameOverSceneName = ""; // laisse vide = reload la scène
    public float respawnDelay = 0.1f;     // petit délai avant de TP

    int lives;            // nb de vies restantes
    Transform player;     // ref auto
    Vector3 startPos;     // position de départ si pas de checkpoint

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) { player = p.transform; startPos = player.position; }

        lives = heartImages.Count; // 3 cœurs => 3 vies
        RefreshUI();
    }

    public void LoseLifeAndRespawn()
    {
        if (!player) return;

        lives = Mathf.Max(0, lives - 1);
        RefreshUI();

        if (lives > 0)
        {
            // respawn au checkpoint (ou au start si pas de checkpoint)
            Vector3 pos = CheckpointManager.HasCheckpoint
                          ? CheckpointManager.LastCheckpoint
                          : startPos;
            StartCoroutine(RespawnCR(pos));
        }
        else
        {
            // plus de vies -> Game Over (reload si nom vide)
            if (string.IsNullOrEmpty(gameOverSceneName))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            else
                SceneManager.LoadScene(gameOverSceneName);
        }
    }

    System.Collections.IEnumerator RespawnCR(Vector3 pos)
    {
        yield return new WaitForSeconds(respawnDelay);

        // remet l’alpha du joueur si tu utilises un FadeOut
        var sr = player.GetComponentInChildren<SpriteRenderer>();
        if (sr) sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);

        // annule vitesse et TP au checkpoint
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.zero;
        player.position = pos;
    }

    void RefreshUI()
    {
        for (int i = 0; i < heartImages.Count; i++)
            heartImages[i].enabled = (i < lives);
    }
}
