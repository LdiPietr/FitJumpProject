using UnityEngine;

public class SpringBoost : PowerUp
{
    public float boostForce = 25f;

    private void Start()
    {
        type = PowerUpType.SpringBoost;
    }

    protected override void Activate(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, boostForce);
    }
}