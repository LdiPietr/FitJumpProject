using UnityEngine;

public class Jetpack : PowerUp
{
    public float flyForce = 15f;

    private void Start()
    {
        type = PowerUpType.Jetpack;
    }

    protected override void Activate(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.ActivateJetpack(duration, flyForce);
    }
}