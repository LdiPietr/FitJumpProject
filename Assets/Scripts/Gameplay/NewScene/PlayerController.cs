using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private float swipeThreshold;
    public float screenPerCent = 2f;
    private Vector3 initialTouchPosition;
    private Vector3 endTouchPosition;

    // Power-up properties
    private bool hasShield;
    private bool hasJetpack;
    private float shieldTimer;
    private float jetpackTimer;
    private float jetpackForce;

    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private float highestY;
    private float lastComboHeight;
    public float comboHeightThreshold = 5f;

    // Shield visual
    public GameObject shieldVisual;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // shieldVisual.SetActive(false);
        swipeThreshold = Screen.width * screenPerCent / 100;
    }

    private void Update()
    {
        HandleMovement();
        HandlePowerUps();
        CheckScreenWrap();
        UpdateScoreAndDifficulty();
    }

    private void HandleMovement()
    {
        if (Input.touchCount == 1) // User is touching the screen with a single touch
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                initialTouchPosition = Input.GetTouch(0).position;
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                endTouchPosition = Input.GetTouch(0).position;

                float deltaX = initialTouchPosition.x - endTouchPosition.x;
                float deltaY = initialTouchPosition.y - endTouchPosition.y;

                float distance = Mathf.Sqrt((deltaX * deltaX) + (deltaY * deltaY));

                if (distance > swipeThreshold)
                {
                    if (deltaX < 0) // Swipe left
                    {
                        MovePlayerLeft();
                    }
                    else if (deltaX > 0) // Swipe right
                    {
                        MovePlayerRight();
                    }
                }
            }
        }

        if (hasJetpack)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jetpackForce);
        }
    }

    void MovePlayerLeft()
    {
        Vector3 newPosition = transform.position;
        newPosition.x -= 5f; // Adjust the distance as needed
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 5f);
    }

    void MovePlayerRight()
    {
        Vector3 newPosition = transform.position;
        newPosition.x += 5f; // Adjust the distance as needed
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 5f);
    }

    private void HandlePowerUps()
    {
        if (hasShield)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0)
            {
                DeactivateShield();
            }
        }

        if (hasJetpack)
        {
            jetpackTimer -= Time.deltaTime;
            if (jetpackTimer <= 0)
            {
                DeactivateJetpack();
            }
        }
    }

    private void CheckScreenWrap()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        if (transform.position.x > screenBounds.x)
        {
            transform.position = new Vector3(-screenBounds.x, transform.position.y, 0);
        }
        else if (transform.position.x < -screenBounds.x)
        {
            transform.position = new Vector3(screenBounds.x, transform.position.y, 0);
        }
    }

    private void UpdateScoreAndDifficulty()
    {
        if (transform.position.y > highestY)
        {
            float heightDifference = transform.position.y - highestY;
            GameplayManager.Instance.AddScore(Mathf.RoundToInt(heightDifference * 10));

            // Aumenta difficoltà in base all'altezza
            GameplayManager.Instance.difficulty = highestY / 50f; // Ogni 50 unità = +1 difficoltà

            // Gestione combo
            if (transform.position.y - lastComboHeight >= comboHeightThreshold)
            {
                GameplayManager.Instance.IncreaseCombo();
                lastComboHeight = transform.position.y;
            }

            highestY = transform.position.y;
        }
        else if (transform.position.y < lastComboHeight - comboHeightThreshold)
        {
            // Reset combo se scende troppo
            GameplayManager.Instance.ResetCombo();
            lastComboHeight = transform.position.y;
        }

        if (transform.position.y < Camera.main.transform.position.y - 5f)
        {
            GameplayManager.Instance.GameOver();
            gameObject.SetActive(false);
        }
    }

    public void ActivateShield(float duration)
    {
        hasShield = true;
        shieldTimer = duration;
        shieldVisual.SetActive(true);
    }

    public void ActivateJetpack(float duration, float force)
    {
        hasJetpack = true;
        jetpackTimer = duration;
        jetpackForce = force;
    }

    public bool HasShield()
    {
        return hasShield;
    }

    private void DeactivateShield()
    {
        hasShield = false;
        shieldVisual.SetActive(false);
    }

    private void DeactivateJetpack()
    {
        hasJetpack = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") && rb.linearVelocity.y <= 0)
        {
            rb.linearVelocity = Vector2.up * jumpForce;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!hasShield)
            {
                GameplayManager.Instance.GameOver();
                gameObject.SetActive(false);
            }
            else
            {
                DeactivateShield();
                Destroy(collision.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            GameplayManager.Instance.GameOver();
            gameObject.SetActive(false);
            Destroy(other.gameObject);
        }
    }
}