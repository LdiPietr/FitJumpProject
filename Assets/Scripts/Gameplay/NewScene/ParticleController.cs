using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public static ParticleController Instance { get; private set; }

    [Header("Particle Systems")]
    public ParticleSystem powerUpEffect;
    public ParticleSystem enemyDeathEffect;
    public ParticleSystem platformBreakEffect;
    public ParticleSystem shieldEffect;
    public ParticleSystem jetpackEffect;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayPowerUpEffect(Vector3 position)
    {
        Instantiate(powerUpEffect, position, Quaternion.identity);
    }

    public void PlayEnemyDeathEffect(Vector3 position)
    {
        Instantiate(enemyDeathEffect, position, Quaternion.identity);
    }

    public void PlayPlatformBreakEffect(Vector3 position)
    {
        Instantiate(platformBreakEffect, position, Quaternion.identity);
    }

    public void PlayShieldEffect(Transform parent)
    {
        ParticleSystem shield = Instantiate(shieldEffect, parent);
        shield.transform.localPosition = Vector3.zero;
    }

    public void PlayJetpackEffect(Transform parent)
    {
        ParticleSystem jetpack = Instantiate(jetpackEffect, parent);
        jetpack.transform.localPosition = new Vector3(0, -0.5f, 0);
    }
}