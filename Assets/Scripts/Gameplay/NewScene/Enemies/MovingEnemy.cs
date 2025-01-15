using UnityEngine;

public class MovingEnemy : Enemy
{
    public float moveSpeed = 3f;
    public float moveDistance = 3f;
    
    private Vector3 startPosition;
    private bool movingRight = true;

    private void Start()
    {
        type = EnemyType.Moving;
        startPosition = transform.position;
    }

    private void Update()
    {
        float movement = moveSpeed * Time.deltaTime;
        if (movingRight)
        {
            if (transform.position.x < startPosition.x + moveDistance)
                transform.Translate(Vector2.right * movement);
            else
                movingRight = false;
        }
        else
        {
            if (transform.position.x > startPosition.x - moveDistance)
                transform.Translate(Vector2.left * movement);
            else
                movingRight = true;
        }
    }
}