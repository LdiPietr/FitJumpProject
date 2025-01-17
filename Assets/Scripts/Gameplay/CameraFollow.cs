using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public float offsetY = 2f;
    public float lowerTolerance = 3f; // Quanto può scendere sotto la posizione minima

    private float highestY;
    private float initialY;

    private void Start()
    {
        highestY = transform.position.y;
        initialY = transform.position.y; // Memorizza la posizione iniziale della telecamera
    }

    private void LateUpdate()
    {
        if (target == null) return;

        float targetY = target.position.y + offsetY;

        // Aggiorna la posizione più alta raggiunta
        if (targetY > highestY)
        {
            highestY = targetY;
        }

        // Calcola la posizione desiderata della telecamera
        float criticalHeight = Mathf.Max(initialY, highestY - lowerTolerance);
        float desiredY = Mathf.Clamp(targetY, criticalHeight, highestY);

        Vector3 desiredPosition = new Vector3(0, desiredY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}
