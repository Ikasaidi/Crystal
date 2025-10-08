using UnityEngine;
 
public class ParallaxBackground : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxEffect = 0.5f;
 
    private Vector3 lastCameraPos;
 
    void Start()
    {
        if (!cameraTransform)
            cameraTransform = Camera.main.transform;
        lastCameraPos = cameraTransform.position;
    }
 
    void Update()
    {
        Vector3 delta = cameraTransform.position - lastCameraPos;
        transform.position += new Vector3(delta.x * parallaxEffect, delta.y * parallaxEffect, 0);
        lastCameraPos = cameraTransform.position;
    }
}