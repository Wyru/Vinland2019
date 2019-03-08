using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Player player;

    public Transform enemyContainer;

    public bool gameOver;

    [Header("Level Config")]
    public bool spawnEnemies;
    public int numberOfEnemies;
    public GameObject[] enemies;
    public GameObject bossPrefab;
    public Transform[] spawnPoints;
    public float timeBtwSpawns;
    public bool spawnAllPoints;
    public bool spawnAfterBoss;
    public bool needKillAllEnemies;

    [Header("Background Musics")]

    public AudioClip minionsMusic;
    public AudioClip bossMusic;
    public AudioClip gameOverMusic;


    private AudioSource audioSource;

    public int numberOfDeadEnemies;

    private bool allEnemiesSpawned;
    private float spawnCooldown;
    private int spawnedEnemies;
    private List<Enemy> enemyList;
    private Enemy boss;

    void Start()
    {
        Instance = this;
        spawnCooldown = timeBtwSpawns;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = minionsMusic;
        audioSource.Play();
        enemyList = new List<Enemy>();

        player.OnCharacterDie += playerDied;

        Invoke("SpawnEnemies", 2f);

    }

    void SpawnEnemies(){
        spawnEnemies = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawnEnemies)
            return;

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

        if(gameOver){
            if(Input.GetKeyDown(KeyCode.Space)){
                RestartLevelAftherDeath();
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
        return enemyList.All(e => e.dead);
    }

    public int CountDeadEnemies()
    {
        return enemyList.Count(e => e.dead);
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
        audioSource.clip = bossMusic;
        audioSource.Play();

        Transform t = spawnPoints[lastSpawnPoint];
        Enemy e = Instantiate(bossPrefab, t.position, Quaternion.identity, enemyContainer).GetComponent<Enemy>();
        boss = e;
    }


    private void playerDied()
    {
        gameOver = true;
        HUDController.Instance.ShowGameOver();
        audioSource.Stop();
        audioSource.clip = gameOverMusic;
        audioSource.loop = false;
        audioSource.Play();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartLevelAftherDeath()
    {
        HUDController.Instance.HideGameOver();
        Invoke("RestartLevel",1f);
    }

}
