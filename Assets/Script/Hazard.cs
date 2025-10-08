using UnityEngine; 
public class Hazard : MonoBehaviour { 
    void OnTriggerEnter2D(Collider2D other) {
         if (!other.CompareTag("Player")) return;
          // déclenche la perte d'une vie + respawn 
          LifeManager.Instance?.LoseLifeAndRespawn(); 
          } 
    }
    