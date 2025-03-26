using UnityEngine;
using System.Collections;

public class DisappearingPlatform : Platform
{
    public float boostForce = 18f;
    public float duration = 0.5f;

    private void Start()
    {
        type = PlatformType.Disappearing;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActive) return;


        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb.linearVelocity.y <= 0)
            {
                AudioManager.Instance.PlaySFX(audioClip);
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, boostForce);
            }
        }
    }
    
}