using System;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public enum PlatformType
    {
        Normal,
        Moving,
        Breakable,
        Disappearing
    }

    public PlatformType type;
    public AudioClip audioClip;
    protected bool isActive = true;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActive) return;


        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb.linearVelocity.y <= 0)
            {
                OnPlayerLand();
            }
        }
    }


    protected virtual void OnPlayerLand()
    {
        // Override nelle classi derivate
    }
}