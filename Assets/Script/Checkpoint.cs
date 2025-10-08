using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        CheckpointManager.SetCheckpoint(transform.position);
        // (optionnel) joue un petit VFX/SFX ici
    }
}
