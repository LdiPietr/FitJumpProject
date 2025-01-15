using UnityEngine;

public class ShootingEnemy : Enemy
{
    public GameObject bulletPrefab;
    public float shootInterval = 2f;
    public float bulletSpeed = 5f;

    private float shootTimer;

    private void Start()
    {
        type = EnemyType.Shooting;
        shootTimer = shootInterval;
    }

    private void Update()
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0)
        {
            Shoot();
            shootTimer = shootInterval;
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Vector2 direction = Vector2.down;
        bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed;
        Destroy(bullet, 3f);
    }
}