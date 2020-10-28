using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private Transform bubblePrefab;
    [SerializeField] private Transform laserPrefab;

    [SerializeField] private int maxActiveBubbles = 10;
    [SerializeField] private float shootCooldown = 0.25f;

    [SerializeField] private float laserCooldown = 4f;
    [SerializeField] private float laserDuration = 2f;

    [SerializeField] private Vector2 playerSpeed;

    [SerializeField] public float maxHunger = 10f;
    [SerializeField] public float hungerRate = 0.005f;
    [SerializeField] public float veggieHungerGain = 2f;
    [SerializeField] public float damageFromEnemy = 2f;
    [SerializeField] public float shootCost = 1f;

    [SerializeField] private Sprite[] sprites;

    private Vector2 playerMovement;

    private Rigidbody2D rigidBodyComponentPlayer;

    private Transform[] bubbles;
    private Rigidbody2D[] bubblesRigidbody2D;
    private int currentBubble = 0;
    private float playerShootCooldown;

    private Color baseColor = Color.white;
    private SpriteRenderer spriteRenderer;

    private float hunger;
    private Slider healthSlider;

    private int score;
    private Text text;

    private bool pumpkinCollected;
    private float shrinkTime;
    private UnityEngine.UI.Image pumpkinUI;

    private Transform laser;
    private float currentLaserCooldown;
    private float laserActiveTime;
    private UnityEngine.UI.Image laserUI;
    private bool laserFiring;

    private void Awake()
    {
        rigidBodyComponentPlayer = GetComponent<Rigidbody2D>();

        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = baseColor;
        healthSlider = GetComponentInChildren<Slider>();
        hunger = maxHunger;
        
        score = 0;
        text = GetComponentInChildren<Text>();
        pumpkinUI = GetComponentsInChildren<UnityEngine.UI.Image>()[5];
        laserUI = GetComponentsInChildren<UnityEngine.UI.Image>()[6];
    }

    private void Start()
    {
        bubbles = new Transform[maxActiveBubbles];
        bubblesRigidbody2D = new Rigidbody2D[maxActiveBubbles];
        for (int i = 0; i < maxActiveBubbles; i++)
        {
            bubbles[i] = Instantiate(bubblePrefab);
            bubblesRigidbody2D[i] = bubbles[i].GetComponent<Rigidbody2D>();
            bubbles[i].gameObject.SetActive(false);
        }
        playerShootCooldown = 0f;

        pumpkinCollected = false;
        shrinkTime = 0;

        laser = Instantiate(laserPrefab);
        laser.SetParent(this.transform, false);
        laser.transform.position = new Vector2(this.transform.position.x + 0.18f, this.transform.position.y - 0.03f);
        laser.gameObject.SetActive(false);
        currentLaserCooldown = laserCooldown;
        laserActiveTime = 0f;
        laserFiring = false;
        laserUI.enabled = false;
    }

    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        spriteRenderer.sprite = sprites[1];
        spriteRenderer.transform.localScale = new Vector3(0.2f, 0.2f, 1);

        playerMovement = new Vector2(playerSpeed.x * inputX, playerSpeed.y * inputY);
        //////////////////////////////////////////////////////////////////
        hunger -= hungerRate;
        healthSlider.value = hunger / maxHunger;
        if (hunger <= 0)
        {
            gameObject.SetActive(false);
            StaticScoreClass.score = score;
            SceneManager.LoadScene("EndScreen");
        }
        //////////////////////////////////////////////////////////////////
        if (playerShootCooldown > 0)
        {
            playerShootCooldown -= Time.deltaTime;
        }
        
        if (Input.GetButton("Fire1") && (playerShootCooldown <= 0f))
        {
            if (bubbles[currentBubble].gameObject.activeSelf)
            {
                bubbles[currentBubble].gameObject.SetActive(false);
            }

            bubbles[currentBubble].position = transform.position;
            bubbles[currentBubble].gameObject.SetActive(true);

            Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            direction.x = Mathf.Clamp(direction.x, -2f, Mathf.Infinity); // FIXME arbitrary min-x to reduce max. aim cone towards front of character
            Vector2 normalized = new Vector2(direction.x, direction.y).normalized;
            bubblesRigidbody2D[currentBubble].velocity = new Vector2(normalized.x * 10f, normalized.y * 10f);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bubbles[currentBubble].GetComponentInChildren<SpriteRenderer>().transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));

            currentBubble++;
            currentBubble %= maxActiveBubbles;
            playerShootCooldown += shootCooldown;
            hunger -= shootCost;
        }
        //////////////////////////////////////////////////////////////////
        text.text = score.ToString();

        if (Input.GetButton("Fire2") && (pumpkinCollected))
        {
            pumpkinCollected = false;
            shrink();
            shrinkTime = 5f;
        }

        if (shrinkTime > 0)
        {
            shrinkTime -= Time.deltaTime;
        }

        if (shrinkTime <= 0)
        {
            grow();
        }

        if (!pumpkinCollected)
        {
            pumpkinUI.enabled = false;
        }
        //////////////////////////////////////////////////////////////////
        if (currentLaserCooldown > 0)
        {
            currentLaserCooldown -= Time.deltaTime;
        }
        else
        {
            laserUI.enabled = true;
        }

        if (Input.GetButton("Fire3") && (currentLaserCooldown <= 0f))
        {
            laserActiveTime = laserDuration;
            currentLaserCooldown = laserCooldown;
            laserUI.enabled = false;
        }

        if (laserActiveTime > 0f)
        {
            laserActiveTime -= Time.deltaTime;
            if (!laserFiring)
            {
                laserFiring = true;
                laser.gameObject.SetActive(true);
            }
        }
        else if (laserFiring)
        {
            laserActiveTime = 0f;
            laserFiring = false;
            laser.gameObject.SetActive(false);
        }
    }
    private void shrink()
    {
        transform.localScale = new Vector3(transform.localScale.x * 0.25f, transform.localScale.y * 0.25f, 1);
    }

    private void grow()
    {
        transform.localScale = new Vector3(1,1,1);
    }

    private void FixedUpdate()
    {
        rigidBodyComponentPlayer.velocity = playerMovement;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("VeggieDone"))
        {
            collision.gameObject.SetActive(false);
            hunger += veggieHungerGain;
            if (hunger > maxHunger) { hunger = maxHunger; }
            score++;
        }
        else if (collision.gameObject.CompareTag("Tomato") || collision.gameObject.CompareTag("Veggie"))
        {
            collision.gameObject.SetActive(false);
            CancelInvoke();

            hunger -= damageFromEnemy;

            colorizeRed();
            Invoke("colorizeBlack", 0.05f);
            Invoke("colorizeRed", 0.1f);
            Invoke("colorizeBlack", 0.15f);
        }
        else if (collision.gameObject.CompareTag("Pumpkin"))
        {
            Destroy(collision.gameObject);
            pumpkinCollected = true;
            pumpkinUI.enabled = true;
        }
    }

    private void colorizeRed()
    {
        spriteRenderer.color = Color.red;
    }

    private void colorizeBlack()
    {
        spriteRenderer.color = baseColor;
    }
}
