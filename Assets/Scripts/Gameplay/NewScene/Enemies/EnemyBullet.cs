using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (!player.HasShield())
            {
                GameplayManager.Instance.GameOver();
            }
            Destroy(gameObject);
        }
    }
}