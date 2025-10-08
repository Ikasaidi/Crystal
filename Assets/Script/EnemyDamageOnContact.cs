// EnemyDamageOnContact.cs
// -----------------------------------------------------------------------------
// À METTRE SUR : l'enfant "Hitbox" de l'ennemi (un GameObject avec un Collider2D)
// - Le "Body" de l'ennemi (parent) garde un Collider2D NON-TRIGGER pour bloquer
//   physiquement le joueur (évite de le traverser).
// - La "Hitbox" (enfant) a un Collider2D avec isTrigger = true et ce script.
// PRÉREQUIS SCÈNE :
// - Le Player a un Rigidbody2D (Dynamic ou Kinematic) + un Collider2D NON-TRIGGER.
// - Le Player porte le Tag "Player" (sur la racine qui a le Rigidbody2D).
// - Physics 2D > Layer Collision Matrix : Player <-> Enemy/Hitbox cochés.
// COMPORTEMENT :
// - Inflige des dégâts au Player à l'entrée dans la hitbox (OnTriggerEnter2D)
//   avec un "cooldown" pour éviter le spam si on re-rentre rapidement.
// - Si tu veux des dégâts en continu pendant le contact, ajoute un OnTriggerStay2D
//   qui réutilise TryHit(...) ou copie la logique (voir NOTE en bas).
// -----------------------------------------------------------------------------

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyDamageOnContact : MonoBehaviour
{
    /// <summary>
    /// Tag attendu sur la racine du joueur (celle qui porte le Rigidbody2D et PlayerHealth).
    /// </summary>
    public string playerTag = "Player";

    /// <summary>
    /// Dégâts infligés à chaque "hit".
    /// </summary>
    public int damage = 10;

    /// <summary>
    /// Délai minimal entre deux coups sur le même joueur (en secondes).
    /// Évite d'infliger plusieurs hits dans le même instant lors d'entrées/sorties rapides.
    /// </summary>
    public float hitCooldown = 0.4f;  // évite le spam

    // Mémorise le dernier moment où un coup a été porté (par ennemi).
    // Pour un cooldown par CIBLE, utiliser un Dictionary< PlayerHealth, float > (voir NOTE).
    float lastHitTime = -999f;

    void Reset()
    {
        // Comme il s'agit d'une hitbox, on force le collider en "trigger"
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // On remonte à l'objet "racine" qui porte le Rigidbody2D (le vrai Player)
        // car le contact peut provenir d'un collider enfant du joueur.
        var root = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;

        // Filtre : on ne réagit qu'au Player (par Tag)
        if (!root.CompareTag(playerTag)) return;

        // Récupère le script de vie du joueur (ta classe à toi)
        var hp = root.GetComponent<PlayerHealth>();
        if (!hp) return; // pas de santé trouvée → rien à faire

        // Anti-spam : si le délai minimal n'est pas écoulé, on ignore ce hit
        if (Time.time - lastHitTime < hitCooldown) return;

        // Inflige les dégâts → ta UI écoute PlayerHealth et mettra la barre à jour
        hp.TakeDamage(damage);

        // DEBUG : vérifie que ça passe ici
        Debug.Log("Hit player -> trigger AttackBat");

        // l'animation attackBat sur l'ennemi (le parent)
        var anim = GetComponentInParent<Animator>();
        if (anim) anim.SetTrigger("AttackBat");

        // Mémorise l'heure du dernier coup
        lastHitTime = Time.time;
    }

    // -------------------------------------------------------------------------
    // NOTE (options selon besoin) :
    //
    // 1) Dégâts en continu pendant qu'on reste dans la hitbox :
    //    - Ajouter :
    //        void OnTriggerStay2D(Collider2D other) { OnTriggerEnter2D(other); }
    //    - ou écrire une méthode TryHit(other) et l'appeler depuis Enter + Stay.
    //
    // 2) Cooldown PAR CIBLE (si plusieurs joueurs / entités soignables existent) :
    //    - Remplacer 'lastHitTime' par :
    //        readonly Dictionary<PlayerHealth, float> _lastHit = new();
    //    - Puis, avant de frapper :
    //        if (_lastHit.TryGetValue(hp, out float last) && Time.time - last < hitCooldown) return;
    //        hp.TakeDamage(damage);
    //        _lastHit[hp] = Time.time;
    //
    // 3) Stopper l'animation de la barre à la sortie de contact :
    //    - Ajouter un OnTriggerExit2D et appeler bar.SnapAndFreeze() sur la UIHealthBar
    //      correspondant au Player (si tu utilises cette feature côté UI).
    // -------------------------------------------------------------------------
}
