using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MonsterScript : MonoBehaviour
{
    [SerializeField] private Transform playerPrefab;
    [SerializeField] private Transform veggiePrefab;
    [SerializeField] private Transform pumpkinPrefab;
    [SerializeField] private int maxActiveVeggies = 10;
    [SerializeField] private float veggieSpawnRate = 1f;
    [SerializeField] private float pumpkinSpawnRate = 10f;

    private Transform player;


    private Transform[] veggies;
    private int currentVeggie = 0;
    private float vegSpawnCooldown;

    private RectTransform spawnArea;

    private float pumpkinCooldown;

    private void Awake()
    {
        player = Instantiate(playerPrefab);
    }

    void Start()
    {
        player.position = new Vector2(-3, 0);

        spawnArea = GetComponent<RectTransform>();
        veggies = new Transform[maxActiveVeggies];

        for (int i = 0; i < maxActiveVeggies; i++)
        {
            veggies[i] = Instantiate(veggiePrefab);
            veggies[i].gameObject.SetActive(false);
        }
        vegSpawnCooldown = 0.5f;

        pumpkinCooldown = 1f;
    }

    void Update()
    {
        if (vegSpawnCooldown > 0)
        {
            vegSpawnCooldown -= Time.deltaTime;
        }

        if (vegSpawnCooldown <= 0f)
        {
            if (veggies[currentVeggie].gameObject.activeSelf)
            {
                veggies[currentVeggie].gameObject.SetActive(false);
            }

            Vector3 position = new Vector3(Random.Range(spawnArea.rect.xMin, spawnArea.rect.xMax), Random.Range(spawnArea.rect.yMin, spawnArea.rect.yMax), 0);
            position += spawnArea.transform.position;
            veggies[currentVeggie].position = position;
            veggies[currentVeggie].gameObject.SetActive(true);
            currentVeggie++;
            currentVeggie %= maxActiveVeggies;
            vegSpawnCooldown += veggieSpawnRate;
        }

        if (pumpkinCooldown > 0)
        {
            pumpkinCooldown -= Time.deltaTime;
        }

        if (pumpkinCooldown <= 0f)
        {
            Transform pumpkin = Instantiate(pumpkinPrefab);
            pumpkin.position = new Vector2(Random.Range(-5f, 5f), Random.Range(-4, 1));
            pumpkinCooldown += pumpkinSpawnRate;
        }
    }
}
