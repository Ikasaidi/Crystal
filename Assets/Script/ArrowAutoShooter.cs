using UnityEngine;

public class ArrowAutoShooter : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= fireRate)
        {
            timer = 0f;
            // on n'utilise pas la rotation du prefab ; on calcule la direction ICI
            Vector2 dir = firePoint ? (Vector2)firePoint.right : Vector2.right;

            var go = Instantiate(arrowPrefab,
                                 firePoint ? firePoint.position : transform.position,
                                 Quaternion.identity);

            var arrow = go.GetComponent<ArrowProjectile>();
            if (!arrow) arrow = go.AddComponent<ArrowProjectile>(); // au cas où
            arrow.Init(dir);
        }
    }
}
