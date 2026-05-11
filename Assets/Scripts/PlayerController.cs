using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class PlayerController : MonoBehaviour
{
    private float elapsedTime = 0f;

    private float score = 0f;
    public float scoreMultiplier = 10f;

    public float thrustForce = 1f;
    public float maxSpeed = 5f;
    public GameObject boosterFlame;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    public UIDocument uiDocument;
    private Label scoreText;

    private bool isDead = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        score = Mathf.FloorToInt(elapsedTime * scoreMultiplier);
        scoreText.text = "Score: " + score;



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
        if (collision.gameObject.CompareTag("Enemy") && !isDead)
        {
            isDead = true;
            rb.simulated = false;
            StartCoroutine(BlinkAndDisappear());
        }
    }

    private System.Collections.IEnumerator BlinkAndDisappear()
    {
        boosterFlame.SetActive(false);
        // Blink 6 times before disappearing // я хз почему не работает я пытался пофиксить
        for (int i = 0; i < 6; i++)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.2f);
        }

        Destroy(gameObject);
    }
}