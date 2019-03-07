using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Player player;

    public Transform enemyContainer;

    [Header("Level Config")]
    public int numberOfEnemies;
    public GameObject[] enemies;
    public GameObject bossPrefab;
    public Transform[] spawnPoints;
    public float timeBtwSpawns;
    public bool spawnAllPoints;
    public bool spawnAfterBoss;
    public bool needKillAllEnemies;


    private bool allEnemiesSpawned;
    private float spawnCooldown;
    private int spawnedEnemies;
    private List<Enemy> enemyList;
    private List<Enemy> deadEnemies;
    private Enemy boss;

    void Start()
    {
        Instance = this;
        spawnCooldown = timeBtwSpawns;
        enemyList = new List<Enemy>();
        deadEnemies = new List<Enemy>();

    }

    // Update is called once per frame
    void Update()
    {
        if (spawnedEnemies < numberOfEnemies)
        {
            if (spawnCooldown > timeBtwSpawns)
            {
                if (spawnAllPoints)
                {
                    SpawnAllPoints();
                }
                else
                {
                    SpawnOnePoint();
                }

                spawnCooldown = 0;
            }
            else
            {
                spawnCooldown += Time.deltaTime;
            }
        }
        else
        {
            allEnemiesSpawned = true;
        }

        if (allEnemiesSpawned && (!needKillAllEnemies || AllEnemiesDead()))
        {
            if (boss == null)
            {
                SpawnBoss();
            }
        }
    }

    private void FixedUpdate()
    {
        InputManager.GetPlayerInputs(player);
    }

    private void SpawnAllPoints()
    {
        foreach (Transform t in spawnPoints)
        {
            SpawnEnemy(t);
        }
    }

    private int lastSpawnPoint = 0;

    private void SpawnOnePoint()
    {
        SpawnEnemy(spawnPoints[lastSpawnPoint]);
        lastSpawnPoint++;
        lastSpawnPoint = lastSpawnPoint % spawnPoints.Length;
    }

    private bool AllEnemiesDead()
    {
        deadEnemies.Clear();
        
        foreach (Enemy e in enemyList)
        {
            if (e.dead)
                deadEnemies.Add(e);
        }

        foreach (Enemy e in deadEnemies)
        {
            enemyList.Remove(e);
        }

        return enemyList.Count == 0;
    }

    private void SpawnEnemy(Transform t)
    {
        int enemy = Random.Range(0, enemies.Length);
        Enemy e = Instantiate(enemies[enemy], t.position, Quaternion.identity, enemyContainer).GetComponent<Enemy>();
        enemyList.Add(e);
        spawnedEnemies++;
    }

    private void SpawnBoss()
    {
        Transform t = spawnPoints[lastSpawnPoint];
        Enemy e = Instantiate(bossPrefab, t.position, Quaternion.identity, enemyContainer).GetComponent<Enemy>();
        boss = e;
    }
}
