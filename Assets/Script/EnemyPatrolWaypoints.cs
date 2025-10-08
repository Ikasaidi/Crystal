using UnityEngine;

// Ces attributs garantissent que l'objet sur lequel ce script est attaché
// possédera toujours un Rigidbody2D et un Collider2D.
// Unity les ajoute automatiquement si on oublie de les mettre.

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyPatrolWaypoints : MonoBehaviour
{
    // Modes de patrouille disponibles
    public enum PatrolMode { Loop, PingPong }

    // États logiques d’animation (à utiliser plus tard avec l’Animator)
    public enum AnimState { Idle, Walk, Run, Attack }

    [Header("Waypoints en ordre")]
    public Transform[] points;             // Liste des points de patrouille

    [Header("Déplacement (patrouille)")]
    public float speed = 2f;               // Vitesse en mode patrouille
    public PatrolMode mode = PatrolMode.Loop; // Mode de déplacement : boucle ou aller-retour
    public float arriveThreshold = 0.2f;   // Distance min pour considérer un waypoint "atteint"
    public float waitAtPoint = 0f;         // Temps d’attente entre deux points

    [Header("Verrous d’axes (top-down)")]
    public bool lockX = false;             // Si true → l’ennemi reste figé en X
    public bool lockY = false;             // Si true → l’ennemi reste figé en Y

    [Header("Détection du joueur")]
    public Transform player;               // Référence au Transform du joueur
    public float walkRadius = 6f;          // Distance → l’ennemi passe en mode WALK vers le joueur
    public float runRadius = 3.5f;         // Distance → l’ennemi passe en mode RUN vers le joueur
    public float attackRadius = 1.2f;      // Distance → l’ennemi passe en mode ATTACK
    public float walkSpeed = 2f;           // Vitesse en mode Walk
    public float runSpeed = 3.5f;          // Vitesse en mode Run
    public float attackCooldown = 1.0f;    // Temps minimal entre deux attaques

    // [Header("Combat")]
    // public int touchDamage = 1;            // Dégâts infligés au joueur

    
    // Références internes
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animator;

    // Variables de gestion de patrouille
    int index = 0;                         // Index du waypoint actuel
    int dirSign = 1;                       // Sens de progression (utile en PingPong)
    float waitTimer = 0f;                  // Timer pour attendre entre 2 points
    float fixedX, fixedY;                  // Valeurs figées si lockX/lockY
    float nextAttackTime = 0f;             // Timer pour le cooldown d’attaque

    // État courant d’animation (logique interne)
    AnimState currentState = AnimState.Idle;

    void Awake()
    {
        // Récupère les composants nécessaires
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Paramètres physiques pour un top-down (pas de gravité, pas de rotation)
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Sauvegarde la position de départ (utile si lockX/lockY activés)
        fixedX = transform.position.x;
        fixedY = transform.position.y;

        // Sécurité : s’il n’y a pas assez de points → désactive le script
        if (points == null || points.Length < 2)
        {
            Debug.LogWarning($"{name}: besoin d’au moins 2 waypoints.");
            enabled = false;
        }

        // État initial = Idle
        SetAnimState(AnimState.Idle);
    }

    void FixedUpdate()
    {
        // Si on est en attente à un waypoint
        if (waitTimer > 0f)
        {
            rb.linearVelocity = Vector2.zero;
            waitTimer -= Time.fixedDeltaTime;

            // Ici on pourrait forcer l’animation en Idle :
            // SetAnimState(AnimState.Idle);
            return;
        }

        // Si un joueur est assigné
        if (player != null)
        {
            float d = Vector2.Distance(rb.position, player.position);

            if (d <= attackRadius)
            {
                // ---- ATTACK ----
                rb.linearVelocity = Vector2.zero;
                animator.SetBool("IsRun", false);
                FaceTarget(player.position);
                SetAnimState(AnimState.Attack);

                // Lancer une attaque toutes les X secondes (cooldown)
                if (Time.time >= nextAttackTime)
                {
                    TryAttack();
                    nextAttackTime = Time.time + attackCooldown;
                }
                return;
            }
            else if (d <= runRadius)
            {
                // ---- RUN ----
                MoveTowards(player.position, runSpeed);
                animator.SetBool("IsRun", true);
                SetAnimState(AnimState.Run);
                return;
            }
            else if (d <= walkRadius)
            {
                // ---- WALK ----
                MoveTowards(player.position, walkSpeed);
                animator.SetBool("IsRun", true);
                SetAnimState(AnimState.Walk);
                return;
            }
            // Sinon → joueur trop loin → patrouille normale
        }

        // ---- PATROUILLE ----
        PatrolStep();
    }

    // Gestion de la patrouille entre waypoints
    void PatrolStep()
    {
        Vector2 target = points[index].position;

        // Applique les verrous si activés
        if (lockX) target.x = fixedX;
        if (lockY) target.y = fixedY;

        Vector2 pos = rb.position;
        Vector2 toTarget = target - pos;
        float dist = toTarget.magnitude;

        if (dist <= arriveThreshold)
        {
            // Waypoint atteint
            AdvanceIndex();
            waitTimer = waitAtPoint;
            rb.linearVelocity = Vector2.zero;

            animator.SetBool("IsRun", false); // Idle
            SetAnimState(AnimState.Idle);
            return;
        }

        // Déplacement vers le waypoint
        MoveTowards(target, speed);
         animator.SetBool("IsRun", true); // ← Run

        // Retourne le sprite (flip) si on change de direction horizontale
        if (sr) sr.flipX = (target.x - pos.x) < 0f;
    }

    // Déplacement vers une cible avec une certaine vitesse
    void MoveTowards(Vector2 target, float moveSpeed)
    {
        Vector2 pos = rb.position;

        if (lockX) target.x = fixedX;
        if (lockY) target.y = fixedY;

        Vector2 dir = (target - pos).normalized;
        Vector2 step = dir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(pos + step);

        // On pourrait ici utiliser le paramètre "Speed" de l’Animator :
        // if (animator) animator.SetFloat("Speed", moveSpeed);

        if (sr && Mathf.Abs(dir.x) > 0.001f) sr.flipX = dir.x < 0f;
    }

    // Passe au waypoint suivant (Loop ou PingPong)
    void AdvanceIndex()
    {
        if (mode == PatrolMode.Loop)
        {
            index = (index + 1) % points.Length;
        }
        else // PingPong
        {
            index += dirSign;
            if (index >= points.Length - 1 || index <= 0) dirSign *= -1;
            index = Mathf.Clamp(index, 0, points.Length - 1);
        }
    }

    // Essaye de faire une attaque (placeholder)
    void TryAttack()
    {
        // Ici tu pourrais lancer un trigger d’animation :
        // if (animator) animator.SetTrigger("Attack");

        // Exemple de dégâts directs :
        // if (player)
        // {
        //     var hp = player.GetComponent<PlayerHealth>();
        //     if (hp) hp.TakeDamage(touchDamage);
        // }
    }

    // void OnCollisionEnter2D(Collision2D col)
    // {
    //     if (!useCollisionDamage) return;      // <- garde-fou

    // if (col.collider.CompareTag("Player"))
    // {
    //     var hp = col.collider.GetComponent<PlayerHealth>();
    //     if (hp) hp.TakeDamage(touchDamage);
    // }
    // }

    // Met à jour l’état d’animation (logique)
    void SetAnimState(AnimState state)
    {
        if (currentState == state) return;
        currentState = state;

        // --- Exemple d’utilisation future avec Animator ---
        // Option 1 : un paramètre int "State"
        // animator.SetInteger("State", (int)currentState);
        //
        // Option 2 : 4 booléens exclusifs "IsIdle", "IsWalk", etc.
        // animator.SetBool("IsIdle",   state == AnimState.Idle);
        // animator.SetBool("IsWalk",   state == AnimState.Walk);
        // animator.SetBool("IsRun",    state == AnimState.Run);
        // animator.SetBool("IsAttack", state == AnimState.Attack);
    }

    // Oriente l’ennemi vers la cible (utile pour Attack)
    void FaceTarget(Vector2 targetPos)
    {
        Vector2 pos = rb.position;
        float dx = targetPos.x - pos.x;
        if (sr && Mathf.Abs(dx) > 0.001f) sr.flipX = dx < 0f;
    }

    // Dessine les gizmos dans l’éditeur
    void OnDrawGizmosSelected()
    {
        // Waypoints
        if (points != null && points.Length > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < points.Length; i++)
            {
                if (!points[i]) continue;
                Gizmos.DrawSphere(points[i].position, 0.08f);

                int next = (i + 1) % points.Length;
                if (mode == PatrolMode.PingPong && i == points.Length - 1) break;
                if (i < points.Length - 1 && points[next])
                    Gizmos.DrawLine(points[i].position, points[next].position);
            }
        }

        // Cercles de détection (Walk, Run, Attack)
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, walkRadius);

        Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, runRadius);

        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
