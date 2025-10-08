using UnityEngine;
using UnityEngine.SceneManagement;
 
public class PlayerHealth : MonoBehaviour
{
    // Nombre de points de vie maximum du joueur (réglable dans l’Inspector).
    public int maxHealth = 5;
 
    // Points de vie courants (privé pour éviter des modifications externes accidentelles).
    private int currentHealth;

    // Pour éviter de mourir
    private bool isDead; 

    private AudioSource audioSource;  // joue les sons
    [SerializeField] private AudioClip sxfdeath;      // son de mort (assigné dans l’Inspector)
    private Animator animator;        
   
 
    // Awake est appelé au chargement du GameObject (avant Start).
    // On initialise les PV courants au maximum.
    void Awake() {
        currentHealth = maxHealth;
        
        //on récupere 
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (!animator)    animator    = GetComponent<Animator>();
        
    }
 
    // Méthode à appeler quand le joueur subit des dégâts.
    // 'dmg' représente la quantité de dégâts à retirer.
    public void TakeDamage(int dmg)
    {
        if(isDead) return; 
        // On soustrait les PV.
        currentHealth -= dmg;
 
        // Log de debug pour visualiser la perte de PV dans la Console.
        Debug.Log("Player prend " + dmg + " dégâts. HP restants = " + currentHealth);
 
        // Si les PV tombent à 0 ou moins, on déclenche la mort.
        if (currentHealth <= 0) Die();
 
        // NOTE :
        // - Tu peux ajouter un Mathf.Clamp pour empêcher les PV de passer sous 0 :
        //   currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        // - Tu peux aussi déclencher ici une mise à jour d’UI (barre de vie).
    }

    
    //l’Animation Event 
    public void OnDeathAnimEvent()
    {
        if (!isDead) Die();
    }

    // Gère la mort du joueur : animations, désactivation d’input, rechargement de scène, etc.
    void Die()
    {
        if (isDead) return;
        isDead = true;

        //Ajout de l'anim dead
        animator.SetTrigger("Dead");
        
        //console
        Debug.Log("Player est mort !");

        //audio 
        audioSource.PlayOneShot(sxfdeath);

        //arreter le joueur et la physique 
        GetComponent<PlayerMove>().enabled = false ;
        GetComponent<Rigidbody2D>().simulated = false; 
        GetComponent<Collider2D>().enabled = false;

        // attendre un peu avant de reload 
        Invoke(nameof(ReloadScene), 0.5f);

    
        // Ici : désactiver le joueur, lancer une animation, recharger la scène, etc.
        // Exemple (selon ton architecture) :
        // GetComponent<PlayerMove>().enabled = false;
        // animator.SetTrigger("Dead");
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ReloadScene(){
        //reload la scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}