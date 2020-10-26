using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegScript : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] private Transform TomatoPrefab;

    [SerializeField] private float shootCooldown = 1.5f;
    [SerializeField] private float actualCooldown;

    [SerializeField] private int maxTomatoes = 10;
    [SerializeField] private Vector2 tomatoMovement = new Vector2(-5f, 0);

    public bool edible;
    
    private bool spawned;

    private Vector2 movement;
    private Rigidbody2D rigidbody2D;

    private SpriteRenderer spriteRenderer;
    private Vector3 growthRate;
    private int veggieType;

    private BoxCollider2D boxCollider2D;

    private Animator animator;

    private Transform[] tomatoes;
    private Rigidbody2D[] tomatoesRigidbody2D;
    private int currentTomatoe = 0;
    private bool firing;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        tomatoes = new Transform[maxTomatoes];
        tomatoesRigidbody2D = new Rigidbody2D[maxTomatoes];
        for (int i = 0; i < maxTomatoes; i++)
        {
            tomatoes[i] = Instantiate(TomatoPrefab);
            tomatoesRigidbody2D[i] = tomatoes[i].GetComponent<Rigidbody2D>();
            tomatoes[i].gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        boxCollider2D.enabled = false;
        growthRate = new Vector3(0.001f, 0.001f, 0);

        veggieType = Random.Range(0, 3);
        spawned = false;
        spriteRenderer.sprite = sprites[veggieType];
        spriteRenderer.color = Color.white;
        transform.localScale = new Vector3(0, 0, 1);

        boxCollider2D.isTrigger = true;
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;

        actualCooldown = Random.Range(0.5f, 1.5f);

        animator.enabled = true;

        edible = false;
        firing = true;
        gameObject.tag = "Veggie";
    }

    private void Update()
    {
        if (!spawned)
        {
            transform.localScale += growthRate;
            if (transform.localScale.x >= 0.15f || transform.localScale.y >= 0.15f)
            {
                spawned = true;
                boxCollider2D.enabled = true;
            }
            movement = new Vector2(-0.5f, 0);
        }

        if (spawned)
        {
            movement = new Vector2(Random.Range(-1,-5), 0);

            if (firing)
            {
                if (actualCooldown > 0f)
                {
                    actualCooldown -= Time.deltaTime;
                }

                if (actualCooldown <= 0f)
                {
                    if (tomatoes[currentTomatoe].gameObject.activeSelf)
                    {
                        tomatoes[currentTomatoe].gameObject.SetActive(false);
                    }

                    tomatoes[currentTomatoe].position = this.transform.position;
                    tomatoes[currentTomatoe].gameObject.SetActive(true);
                    tomatoesRigidbody2D[currentTomatoe].velocity = tomatoMovement;

                    currentTomatoe++;
                    currentTomatoe %= maxTomatoes;
                    actualCooldown += shootCooldown;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (rigidbody2D.bodyType == RigidbodyType2D.Dynamic)
        {
            rigidbody2D.velocity = movement;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bubble"))
        {
            collision.gameObject.SetActive(false);
            veggieDown();
        }
    }

    public void veggieDown()
    {
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        animator.enabled = false;
        edible = true;
        firing = false;
        gameObject.tag = "VeggieDone";

        spriteRenderer.color = new Color(
           Random.Range(140, 160) / 255f,
           Random.Range(110, 130) / 255f,
           Random.Range(70, 90) / 255f,
           1
       );
    }
}
