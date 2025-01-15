// Versione aggiornata con effetti e audio

using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        Basic,
        Moving,
        Shooting
    }

    public EnemyType type;
    public float damage = 1f;
    public int scoreValue = 100;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            
            if (collision.contacts[0].normal.y > 0.7f)
            {
                Die();
                player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * player.jumpForce;
            }
            else
            {
                HandlePlayerCollision(player);
            }
        }
    }

    protected virtual void HandlePlayerCollision(PlayerController player)
    {
        if (player.HasShield())
        {
            Die();
        }
        else
        {
            GameplayManager.Instance.GameOver();
        }
    }

    protected virtual void Die()
    {
        GameplayManager.Instance.AddScore(scoreValue);
        ParticleController.Instance.PlayEnemyDeathEffect(transform.position);
        Destroy(gameObject);
    }
}