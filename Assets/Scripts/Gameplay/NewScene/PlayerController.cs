using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    
    public enum TouchPhase
    {
        Start,
        Stop
    }

    public TouchPhase touchPhase = TouchPhase.Stop;

    public float speed = 19.5f;
    public float lastPosition = 0.0f;
    public float tolerance = 0.1f;
    public float moveInput;

    
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
        //shieldVisual.SetActive(false);
    }

    private void Update()
    {
        HandleMovement();
        HandlePowerUps();
        CheckScreenWrap();
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

    private void HandleMovement()
    {
        if (Input.GetMouseButton(0))
        {
            if (touchPhase == TouchPhase.Stop)
            {
                touchPhase = TouchPhase.Start;
                lastPosition = Input.mousePosition.x;
            }
            else
            {
                var delta = Input.mousePosition.x - lastPosition;
                if (Mathf.Abs(delta) > tolerance)
                {
                    var screen = Screen.width * 0.00001f;
                    moveInput = delta;
                    lastPosition = Input.mousePosition.x;
                    
                    // Calcola la posizione target
                    float targetX = transform.position.x + moveInput * screen * speed;
                    
                    // Interpola in modo fluido verso la posizione target
                    float smoothX = Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * moveSpeed);
                    
                    transform.position = new Vector3(smoothX, transform.position.y, transform.position.z);
                }
                else
                {
                    moveInput = 0.0f;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            touchPhase = TouchPhase.Stop;
            moveInput = 0.0f;
        }
    
        if (hasJetpack)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jetpackForce);
        }
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
