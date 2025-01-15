using UnityEngine;

public class MovingPlatform : Platform
{
    public float moveSpeed = 2f;
    public float moveDistance = 2f;
    
    private Vector3 startPosition;
    private bool movingRight = true;

    private void Start()
    {
        type = PlatformType.Moving;
        startPosition = transform.position;
    }

    private void Update()
    {
        if (!isActive) return;

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