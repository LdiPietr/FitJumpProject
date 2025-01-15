using UnityEngine;

public class DisappearingPlatform : Platform
{
    public float disappearDelay = 0.5f;
    public float reappearDelay = 2f;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private void Start()
    {
        type = PlatformType.Disappearing;
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    protected override void OnPlayerLand()
    {
        if (!isActive) return;
        
        isActive = false;
        Invoke("Disappear", disappearDelay);
    }

    private void Disappear()
    {
        spriteRenderer.enabled = false;
        boxCollider.enabled = false;
        Invoke("Reappear", reappearDelay);
    }

    private void Reappear()
    {
        spriteRenderer.enabled = true;
        boxCollider.enabled = true;
        isActive = true;
    }
}