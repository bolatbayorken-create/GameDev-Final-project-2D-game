using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


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

    public GameObject explosionEffect;
    private Button restartButton;

    public GameObject borderParent;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        highScoreText = uiDocument.rootVisualElement.Q<Label>("HighScoreLabel");
        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");

        restartButton.style.display = DisplayStyle.None;
        restartButton.clicked += ReloadScene;

        highScore = PlayerPrefs.GetFloat("HighScore", 0f);
        if (highScoreText != null) highScoreText.text = "Best: " + highScore;
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

        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
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
        if (collision.gameObject.CompareTag("Enemy"))
        {
            rb.simulated = false;
            // StartCoroutine(BlinkAndDisappear());
            Instantiate(explosionEffect, transform.position, transform.rotation);
            Destroy(gameObject);
            restartButton.style.display = DisplayStyle.Flex;
        }
        borderParent.SetActive(false);
    }
    
    /*
    private System.Collections.IEnumerator BlinkAndDisappear()
    {
        boosterFlame.SetActive(false);
        // Blink 6 times before disappearing // я хз почему не работает я пытался пофиксить
        for (int i = 0; i < 6; i++)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.2f);
        }
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(gameObject);
        restartButton.style.display = DisplayStyle.Flex;
    }
    */

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}