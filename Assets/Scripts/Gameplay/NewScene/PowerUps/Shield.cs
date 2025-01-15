using UnityEngine;

public class Shield : PowerUp
{
    private void Start()
    {
        type = PowerUpType.Shield;
    }

    protected override void Activate(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.ActivateShield(duration);
    }
}