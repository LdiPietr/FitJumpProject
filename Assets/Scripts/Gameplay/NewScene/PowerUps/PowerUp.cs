using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType
    {
        Jetpack,
        Shield,
        SpringBoost
    }

    public PowerUpType type;
    public float duration = 5f;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Activate(other.gameObject);
            Destroy(gameObject);
        }
    }

    protected virtual void Activate(GameObject player)
    {
        // Override nelle classi derivate
    }
}