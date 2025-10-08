using UnityEngine;

[RequireComponent(typeof(PlayerEffects))]
public class PlayerMove : MonoBehaviour
{
    // Sons
    [SerializeField] AudioClip sfxJump;
    [SerializeField] AudioClip sfxAttack;

    // Déplacement
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float jumpSpeed = 12f;

    // Double jump
    [Header("Sauts")]
    [SerializeField] int maxJumps = 2;     // ← 2 = double-jump
    int jumpsLeft;

    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;
    private PlayerEffects effects;

    private float x;
    private bool jump = false;

    // Sol (garde tes valeurs)
    [SerializeField] LayerMask groundMask;
    [SerializeField] Vector2 groundCheckOffset = new(0f, -0.5f);
    [SerializeField] float groundCheckRadius = 0.15f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator       = GetComponent<Animator>();
        rb             = GetComponent<Rigidbody2D>();
        audioSource    = GetComponent<AudioSource>();
        effects        = GetComponent<PlayerEffects>();

        rb.freezeRotation = true;
        if (rb.gravityScale < 2.5f) rb.gravityScale = 2.5f;

        // prêt pour un double-jump
        jumpsLeft = maxJumps;
    }

    void Update()
    {
        // ---- Déplacement horizontal ----
        x = Input.GetAxis("Horizontal");
        if (effects != null) x *= effects.inputSign;

        animator.SetFloat("x", Mathf.Abs(x));
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime * x);

        // ---- Orientation ----
        if (x > 0f) spriteRenderer.flipX = false;
        if (x < 0f) spriteRenderer.flipX = true;

        // ---- (Ré)initialiser les sauts quand on est au sol ----
        if (IsGrounded() && rb.linearVelocity.y <= 0.01f)
        {
            jumpsLeft = maxJumps;
            animator.SetBool("Jump", false);
        }

        // ---- Demande de saut (double jump inclus) ----
        if (Input.GetKeyDown(KeyCode.UpArrow) && jumpsLeft > 0)
        {
            jump = true;           // on sautera en FixedUpdate
            jumpsLeft--;           // consomme un saut
            animator.SetBool("Jump", true);
            if (sfxJump) audioSource.PlayOneShot(sfxJump);
        }

        // ---- Attaque ----
        if (Input.GetKey(KeyCode.Space))
        {
            animator.SetBool("Attack", true);
            if (sfxAttack) audioSource.PlayOneShot(sfxAttack);
        }
        else
        {
            animator.SetBool("Attack", false);
        }
    }

    private void FixedUpdate()
    {
        // pas de translate ici
        if (jump)
        {
            jump = false;
            // saut franc : on remplace juste la vitesse verticale
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
        }
    }

    // ---- helpers ----
    private bool IsGrounded()
    {
        if (groundMask == 0) return false; // évite les sauts infinis si non réglé
        Vector2 p = (Vector2)transform.position + groundCheckOffset;
        return Physics2D.OverlapCircle(p, groundCheckRadius, groundMask) != null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 p = (Vector2)transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(p, groundCheckRadius);
    }
}
