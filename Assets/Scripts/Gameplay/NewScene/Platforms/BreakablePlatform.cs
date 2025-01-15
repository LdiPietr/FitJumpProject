using UnityEngine;

public class BreakablePlatform : Platform
{
    public float breakDelay = 0.1f;

    private void Start()
    {
        type = PlatformType.Breakable;
    }

    protected override void OnPlayerLand()
    {
        if (!isActive) return;
        
        isActive = false;
        Invoke("Break", breakDelay);
    }

    private void Break()
    {
        // Qui puoi aggiungere effetti particellari
        Destroy(gameObject);
    }
}