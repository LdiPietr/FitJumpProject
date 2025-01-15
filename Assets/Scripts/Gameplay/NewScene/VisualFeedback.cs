using UnityEngine;

public class VisualFeedback : MonoBehaviour
{
    public static VisualFeedback Instance { get; private set; }

    [Header("Screen Shake")]
    public float shakeDuration = 0.2f;
    public float shakeIntensity = 0.1f;
    
    [Header("Flash Effects")]
    public Material flashMaterial;
    public float flashDuration = 0.1f;

    private Camera mainCamera;
    private Vector3 originalCameraPosition;
    private float shakeTimer;

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
        originalCameraPosition = mainCamera.transform.localPosition;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            Vector3 randomOffset = Random.insideUnitSphere * shakeIntensity;
            mainCamera.transform.localPosition = originalCameraPosition + randomOffset;

            if (shakeTimer <= 0)
            {
                mainCamera.transform.localPosition = originalCameraPosition;
            }
        }
    }

    public void ShakeCamera()
    {
        shakeTimer = shakeDuration;
    }

    public void FlashSprite(SpriteRenderer sprite)
    {
        StartCoroutine(FlashRoutine(sprite));
    }

    private System.Collections.IEnumerator FlashRoutine(SpriteRenderer sprite)
    {
        Material originalMaterial = sprite.material;
        sprite.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        sprite.material = originalMaterial;
    }
}