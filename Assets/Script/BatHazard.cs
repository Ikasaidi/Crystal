using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class BatHazard : MonoBehaviour
{
    [SerializeField] float cooldown = 0.4f; // empêche les multi-hits
    Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true; // sécurité
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // -1 vie + respawn (géré par ton LifeManager)
        LifeManager.Instance?.LoseLifeAndRespawn();

        // bloque ce hazard un court instant pour ne PAS re-compter
        StartCoroutine(TempDisable());
    }

    IEnumerator TempDisable()
    {
        col.enabled = false;
        yield return new WaitForSeconds(cooldown);
        col.enabled = true;
    }
}
