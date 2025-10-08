using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove_L1 : MonoBehaviour
{
    [Header("Déplacement")]
    public float moveSpeed = 4f;
    public float jumpSpeed = 12f;

    [Header("Sauts")]
    public int maxJumps = 2;
    public LayerMask groundMask;
    public Vector2 groundCheckOffset = new(0f, -0.5f);
    public float groundCheckRadius = 0.15f;

    [Header("Audio")]
    [SerializeField] AudioClip sfxJump;
    [SerializeField] AudioClip sfxAttack;
    [SerializeField] AudioClip sfxDeath;

    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;

    private float x;
    private int jumpsLeft;
    private bool jumpPressed;
    private bool isDead = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator       = GetComponent<Animator>();
        rb             = GetComponent<Rigidbody2D>();
        audioSource    = GetComponent<AudioSource>();

        if (rb)
        {
            rb.freezeRotation = true;
            if (rb.gravityScale < 2.5f) rb.gravityScale = 2.5f;
        }

        jumpsLeft = maxJumps;
    }

    void Update()
    {
        if (isDead) return; // bloque tout si mort

        // ---- Déplacement horizontal ----
        x = Input.GetAxis("Horizontal");
        animator?.SetFloat("x", Mathf.Abs(x));

        // ---- Orientation ----
        if (x > 0.01f) spriteRenderer.flipX = false;
        if (x < -0.01f) spriteRenderer.flipX = true;

        // ---- Sauts ----
        if (IsGrounded() && rb.linearVelocity.y <= 0.01f)
        {
            jumpsLeft = maxJumps;
            animator?.SetBool("Jump", false);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && jumpsLeft > 0)
        {
            jumpPressed = true;
            jumpsLeft--;
            animator?.SetBool("Jump", true);
            PlaySound(sfxJump);
        }

        // ---- Attaque ----
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator?.SetBool("Attack", true);
            PlaySound(sfxAttack);
        }
        else
        {
            animator?.SetBool("Attack", false);
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        // Déplacement horizontal
        transform.Translate(Vector2.right * moveSpeed * Time.fixedDeltaTime * x);

        // Saut
        if (jumpPressed)
        {
            jumpPressed = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
        }
    }

    bool IsGrounded()
    {
        if (groundMask == 0) return true;
        Vector2 p = (Vector2)transform.position + groundCheckOffset;
        return Physics2D.OverlapCircle(p, groundCheckRadius, groundMask) != null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 p = (Vector2)transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(p, groundCheckRadius);
    }

    // --- Détection de mort ---
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hazard") && !isDead)
        {
            isDead = true;
            // animator?.SetTrigger("Death");
            // PlaySound(sfxDeath);
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            Invoke(nameof(RestartLevel), 2f); // attend 2s avant de relancer
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}
