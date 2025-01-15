using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public float offsetY = 2f;
    public float lowerTolerance = 3f;

    private float highestY;

    private void Start()
    {
        highestY = transform.position.y;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        float targetY = target.position.y + offsetY;
        
        // La camera sale solo se il target va più in alto del punto più alto raggiunto
        if (targetY > highestY)
        {
            highestY = targetY;
        }

        // La posizione Y della camera non scenderà mai sotto highestY
        Vector3 desiredPosition = new Vector3(0, highestY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}