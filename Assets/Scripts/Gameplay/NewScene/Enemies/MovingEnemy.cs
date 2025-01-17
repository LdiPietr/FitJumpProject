using UnityEngine;

public class MovingEnemy : Enemy
{
    public float minMoveSpeed = 2f;
    public float maxMoveSpeed = 5f;
    public float minMoveDistance = 2f;
    public float maxMoveDistance = 5f;

    private float moveSpeed;
    private float moveDistance;

    private Vector3 startPosition;
    private bool movingRight = true;

    private void Start()
    {
        type = EnemyType.Moving;
        startPosition = transform.position;

        // Assegna valori casuali all'interno degli intervalli
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        moveDistance = Random.Range(minMoveDistance, maxMoveDistance);
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
