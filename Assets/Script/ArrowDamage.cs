using UnityEngine;

public class ArrowDamage : MonoBehaviour
{
    public int damage = 1; // dégâts infligés
    public string playerTag = "Player";
    // son damage
    [SerializeField] public AudioClip sfxDamage;

    void Reset()
    {
        // s’assure que le collider est en mode trigger
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Vérifie si on touche le joueur
        if (!other.CompareTag(playerTag)) return;

        // Cherche le script de vie du joueur
        var hp = other.GetComponent<PlayerHealth>();
        if (!hp) return;

        // Inflige les dégâts
        hp.TakeDamage(damage);

        if (sfxDamage) AudioSource.PlayClipAtPoint(sfxDamage, transform.position);

        // Détruit la flèche après impact
        Destroy(gameObject);
    }
}
