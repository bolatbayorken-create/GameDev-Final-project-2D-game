using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;               
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
    private UnityEngine.UIElements.Button restartButton; 

    public GameObject explosionEffect;
    public GameObject borderParent;

    public InputAction moveForward;
    public InputAction lookPosition;

    public UnityEngine.UI.Image gameOverImage; // эм

    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        highScoreText = uiDocument.rootVisualElement.Q<Label>("HighScoreLabel");
        restartButton = uiDocument.rootVisualElement.Q<UnityEngine.UIElements.Button>("RestartButton");

        restartButton.style.display = DisplayStyle.None;
        restartButton.clicked += ReloadScene;

        highScore = PlayerPrefs.GetFloat("HighScore", 0f);
        if (highScoreText != null) highScoreText.text = "Best: " + highScore;

        moveForward.Enable();
        lookPosition.Enable();

        if (gameOverImage != null)
        {
            gameOverImage.gameObject.SetActive(false);
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
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isDead)
        {
            isDead = true;
            rb.simulated = false;
            Instantiate(explosionEffect, transform.position, transform.rotation);
            Destroy(gameObject);
            restartButton.style.display = DisplayStyle.Flex;
            if (borderParent != null) borderParent.SetActive(false);
            if (gameOverImage != null) gameOverImage.gameObject.SetActive(true);
        }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}