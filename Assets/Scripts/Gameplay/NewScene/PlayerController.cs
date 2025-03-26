using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public Sprite[] sprites;
    public Sprite super;
    public SpriteRenderer spriteRenderer;
    public float moveSpeed = 10f;
    public float jumpForce = 10f;

    // Power-up properties
    private bool hasShield;
    private bool hasJetpack;
    public float shieldTimer;
    private float jetpackTimer;
    private float jetpackForce;

    // Components
    private Rigidbody2D rb;

    private float highestY;
    private float lastComboHeight;
    public float comboHeightThreshold = 5f;

    // Shield visual
    public GameObject shieldVisual;
    public GameObject jetpackVisual;

    private bool gameStarted = false;
    public float countdownDuration = 3f;

    private float screenHalfWidthInWorldUnits;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<BoxCollider2D>().enabled = false;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Calcola la metà della larghezza dello schermo in unità del mondo
        float halfPlayerWidth = transform.localScale.x / 2f;
        screenHalfWidthInWorldUnits = Camera.main.aspect * Camera.main.orthographicSize - halfPlayerWidth;

        StartCoroutine(StartGameCountdown());
    }

    private IEnumerator StartGameCountdown()
    {
        // Disabilita il movimento impostando la velocità a zero
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0; // Disabilita la gravità durante il countdown

        // Chiedi all'UI Manager di mostrare il countdown
        InGameUIManager.Instance.ShowCountdown(countdownDuration);

        // Attendi per la durata del countdown
        yield return new WaitUntil(() => !InGameUIManager.Instance.countdownPanel.activeSelf);

        GetComponent<BoxCollider2D>().enabled = true;
        // Abilita il movimento
        gameStarted = true;
        rb.gravityScale = 1; // Ripristina la gravità

        // Nascondi il countdown
        InGameUIManager.Instance.HideCountdown();
    }

    private void Update()
    {
        if (!gameStarted) return;

        HandleMovement();
        HandlePowerUps();
        CheckScreenWrap();

        if (transform.position.y > highestY)
        {
            float heightDifference = transform.position.y - highestY;
            GameplayManager.Instance.AddScore(Mathf.RoundToInt(heightDifference * 10));

            // Aumenta difficoltà in base all'altezza
            GameplayManager.Instance.difficulty = highestY / 50f;

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

        if (jetpackTimer > 0)
        {
            spriteRenderer.sprite = super;
        }
        else
        {
            spriteRenderer.sprite = rb.linearVelocity.y < -0.5f ? sprites[3] : sprites[0];
        }
    }

    private void HandleMovement()
    {
        if (!gameStarted) return;

        // Ottieni l'input dell'accelerometro
        Vector3 acceleration = Input.acceleration;

        // Calcola il movimento basato sull'inclinazione del dispositivo
        float movement = acceleration.x * moveSpeed * Time.deltaTime;

        // Muovi il giocatore in base all'inclinazione
        transform.Translate(Vector2.right * movement);

        // Limita il movimento del giocatore ai bordi dello schermo
        float clampedX = Mathf.Clamp(transform.position.x, -screenHalfWidthInWorldUnits, screenHalfWidthInWorldUnits);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);

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
        StartCoroutine(shieldVisual.GetComponent<VisualShield>().UpdateShield());
        AudioManager.Instance.PlaySFX(AudioManager.Instance.shield);
    }

    public void ActivateJetpack(float duration, float force)
    {
        hasJetpack = true;
        jetpackTimer = duration;
        jetpackForce = force;
        jetpackVisual.SetActive(true);
        AudioManager.Instance.PlaySFX(AudioManager.Instance.jet);
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
        jetpackVisual.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") && rb.linearVelocity.y <= 0)
        {
            rb.linearVelocity = Vector2.up * jumpForce;
            AudioManager.Instance.PlaySFX(collision.gameObject.GetComponent<Platform>().audioClip);
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