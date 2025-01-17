using UnityEngine;
using System.Collections;

public class DisappearingPlatform : Platform
{
    public float minVisibleDuration = 1f;
    public float maxVisibleDuration = 3f;
    public float minInvisibleDuration = 0.5f;
    public float maxInvisibleDuration = 1.5f;

    private float visibleDuration;
    private float invisibleDuration;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private bool isVisible = true;

    private void Start()
    {
        type = PlatformType.Disappearing;
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Assegna durate casuali all'interno degli intervalli
        visibleDuration = Random.Range(minVisibleDuration, maxVisibleDuration);
        invisibleDuration = Random.Range(minInvisibleDuration, maxInvisibleDuration);

        StartCoroutine(CycleVisibility());
    }

    private IEnumerator CycleVisibility()
    {
        while (true)
        {
            if (isVisible)
            {
                // La piattaforma è visibile
                boxCollider.enabled = true;
                SetAlpha(1f);
                yield return new WaitForSeconds(visibleDuration);
            }
            else
            {
                // La piattaforma è invisibile
                boxCollider.enabled = false;
                SetAlpha(0f);
                yield return new WaitForSeconds(invisibleDuration);
            }
            isVisible = !isVisible;
        }
    }

    private void SetAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
}
