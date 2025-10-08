using UnityEngine;

public class AcidKill : MonoBehaviour
{
    public GameObject player;
    public string acidTag = "Droop"; // mets "Acid" si tu préfères
    private Animator playerAnimator;

 
    void Start()
    {
       
            playerAnimator = player.GetComponent<Animator>();
        
        
    }
 
    void OnTriggerEnter2D(Collider2D other)
    {
        
 
        if (other.CompareTag("Player")){
            playerAnimator.SetTrigger("Dead"); // tu avais déjà ce trigger :contentReference[oaicite:3]{index=3}
            
        }
        
    }
 
}
