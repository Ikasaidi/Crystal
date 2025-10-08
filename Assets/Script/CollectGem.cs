using UnityEngine;

public class CollectGem : MonoBehaviour
{
    [Header("Pickup")]
    [SerializeField] AudioClip sfxPickup;
    [SerializeField] bool cursed = false;      // coche dans l’Inspector si gemme maudite
    [SerializeField] float cursedDuration = 3f;

    [Header("VFX (optionnel)")]
    [SerializeField] GameObject normalVFX;
    [SerializeField] GameObject cursedVFX;

    bool consumed = false; // évite double ramassage

    void OnTriggerEnter2D(Collider2D other)
    {
        if (consumed) return;
        if (!other.CompareTag("Player")) return;

        consumed = true;

        // Son
        if (sfxPickup) AudioSource.PlayClipAtPoint(sfxPickup, transform.position);

        // Effets
        if (cursed)
        {
            var pe = other.GetComponent<PlayerEffects>();
            if (pe) pe.ApplyCursed(cursedDuration);
            if (cursedVFX) Instantiate(cursedVFX, transform.position, Quaternion.identity);
        }
        else
        {
            // TODO: incrémenter le score ici
            if (normalVFX) Instantiate(normalVFX, transform.position, Quaternion.identity);
        }

        // : compter la gemme
        GemGoalManager.Instance?.AddGem(1);

        // Disparition de la gemme
        Destroy(gameObject);
        // (ou gameObject.SetActive(false); si tu préfères la réactiver plus tard)
    }
}
