using System.Collections;
using UnityEngine;

public class BreakablePlatform : Platform
{
    public float breakDelay = 0.1f;
    public Sprite[] breakSprites;
    private SpriteRenderer spriteRenderer;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        type = PlatformType.Breakable;
    }

    protected override void OnPlayerLand()
    {
        if (!isActive) return;

        isActive = false;
        StartCoroutine(Break());
    }

    private IEnumerator Break()
    {
        spriteRenderer.sprite = breakSprites[0];
        yield return new WaitForSeconds(breakDelay);
        GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(breakDelay);
        spriteRenderer.sprite = breakSprites[1];
        yield return new WaitForSeconds(breakDelay);
        Destroy(gameObject);
    }
}