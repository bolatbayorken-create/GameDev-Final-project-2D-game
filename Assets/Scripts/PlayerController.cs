using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private float elapsedTime = 0f;
    private float score = 0f;
    public float scoreMultiplier = 10f;
    private float highScore = 0f;

    public float thrustForce = 1f;
    public float maxSpeed = 5f;
    public GameObject boosterFlame;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    public UIDocument uiDocument;
    private Label scoreText;
    private Label highScoreText;
    private Button restartButton;

    public GameObject explosionEffect;
    public GameObject borderParent;

    public InputAction moveForward;
    public InputAction lookPosition;

    public UnityEngine.UI.Image gameOverImage; // эм

    private VisualElement pauseMenu;
    private Button resumeButton;
    private bool isPaused = false;

    private Button mainMenuButton;

    public int maxLives = 3;
    private int currentLives;

    public float invulnerabilityTime = 1.5f;
    private float invulnTimer = 0f;
    private bool isInvulnerable = false;

    private Label healthText;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        highScoreText = uiDocument.rootVisualElement.Q<Label>("HighScoreLabel");
        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        pauseMenu = uiDocument.rootVisualElement.Q<VisualElement>("pauseMenu");
        resumeButton = uiDocument.rootVisualElement.Q<Button>("ResumeButton");
        mainMenuButton = uiDocument.rootVisualElement.Q<Button>("mainMenuButton");

        pauseMenu.style.display = DisplayStyle.None;
        restartButton.clicked += ReloadScene;

        highScore = PlayerPrefs.GetFloat("HighScore", 0f);
        if (highScoreText != null) highScoreText.text = "Best: " + highScore;

        moveForward.Enable();
        lookPosition.Enable();

        currentLives = maxLives;
        healthText = uiDocument.rootVisualElement.Q<Label>("HealthLabel");
        if (healthText != null) healthText.text = "Lives: " + currentLives;

        if (gameOverImage != null)
        {
            gameOverImage.gameObject.SetActive(false);
        }

        if (resumeButton != null)
        {
            resumeButton.clicked += TogglePause;
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.clicked += () => {
                Debug.Log("pressed");
                Time.timeScale = 1f;
                SceneManager.LoadScene("MainMenu");
            };
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        score = Mathf.FloorToInt(elapsedTime * scoreMultiplier);
        scoreText.text = "Score: " + score;

        if (score > highScore)
        {
            highScore = score;
            if (highScoreText != null) highScoreText.text = "Best: " + highScore;
            PlayerPrefs.SetFloat("HighScore", highScore);
        }

        if (moveForward.IsPressed())
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(lookPosition.ReadValue<Vector2>());
            Vector2 direction = (mousePos - transform.position).normalized;

            transform.up = direction;
            rb.AddForce(direction * thrustForce);

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            boosterFlame.SetActive(true);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            boosterFlame.SetActive(false);
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
        if (isInvulnerable)
        {
            invulnTimer -= Time.deltaTime;


            sr.enabled = (Mathf.FloorToInt(invulnTimer * 10f) % 2 == 0);

            if (invulnTimer <= 0f)
            {
                isInvulnerable = false;
                sr.enabled = true;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isInvulnerable)
        {
            currentLives--;
            if (healthText != null) healthText.text = "Lives: " + currentLives;

            if (currentLives <= 0)
            {
                rb.simulated = false;
                var col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;
                Instantiate(explosionEffect, transform.position, transform.rotation);
                Destroy(gameObject);
                if (boosterFlame != null) boosterFlame.SetActive(false); // Выключаем пламя двигателя
                if (pauseMenu != null)
                {
                    pauseMenu.style.display = DisplayStyle.Flex;
                }
                if (resumeButton != null)
                {
                    resumeButton.style.display = DisplayStyle.None;
                }
                if (borderParent != null) borderParent.SetActive(false);
                if (gameOverImage != null) gameOverImage.gameObject.SetActive(true);
            }
            else
            {
                isInvulnerable = true;
                invulnTimer = invulnerabilityTime;
            }
        }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pauseMenu != null) pauseMenu.style.display = DisplayStyle.Flex;
        }
        else
        {
            Time.timeScale = 1f;
            if (pauseMenu != null) pauseMenu.style.display = DisplayStyle.None;

        }
    }
}