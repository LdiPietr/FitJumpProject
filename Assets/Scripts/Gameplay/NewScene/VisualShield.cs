using System.Collections;
using UnityEngine;

public class VisualShield : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public IEnumerator UpdateShield()
    {
        yield return new WaitUntil(() => PlayerController.Instance.shieldTimer < 1.5f);
        while (PlayerController.Instance.shieldTimer is > 0 and < 1.5f)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
