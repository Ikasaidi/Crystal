// ArrowProjectile.cs
using UnityEngine;


public class ArrowProjectile : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 5f;
    public float spawnOffset = 0.3f;

    private Rigidbody2D rb;
    private Vector2 dir;

    public void Init(Vector2 direction)
    {
        rb = GetComponent<Rigidbody2D>();
        dir = direction.normalized;

        transform.right = dir;
        transform.position += (Vector3)(dir * spawnOffset); // évite de toucher le mur direct

        rb.linearVelocity = dir * speed;
        Destroy(gameObject, lifeTime); // reste visible 5s
    }
}
