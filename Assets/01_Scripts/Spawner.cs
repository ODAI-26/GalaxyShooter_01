using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{

    public float timeBtwSpawn = 1.5f;
    float timer = 0;
    public Transform leftPoint;
    public Transform rightPoint;
    public List<GameObject> enemyPrefabs;
    public int score = 0;
    public Text scoreText;
    public static Spawner instance; //Singleton, para guardar referencia del script
    public GameObject enemyBossPrefab;

    public int wave = 1;
    private bool bossIsSpawned = false;
    public Text waveText;
    
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }//examen
    }
    void Start()
    {
        scoreText.text = "SCORE: " + score;
        waveText.text = "WAVE: "+wave;
    }

    void Update()
    {
        if(wave < 4)
        {
            SpawnEnemy();
        }
        else if(wave==4 && !bossIsSpawned)
        {
            SpawnBoss();
        }
        
    }

    void SpawnEnemy()
    {
        if (timer < timeBtwSpawn)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            float x = Random.Range(leftPoint.position.x, rightPoint.position.x);
            int enemy = Random.Range(0, enemyPrefabs.Count);
            Vector3 newPos = new Vector3(x, transform.position.y, transform.position.z);
            Instantiate(enemyPrefabs[enemy], newPos, Quaternion.Euler(0, 180, 0));
        }
    }
    public void AddScore(int points)
    {
        score+=points;
        scoreText.text = "SCORE: " + score;
        AddWave();
    }

    void SpawnBoss()
    {
        Instantiate(enemyBossPrefab, new Vector3(0, 5, 0), Quaternion.identity); 
        waveText.text = "ENEMY BOSS";
        bossIsSpawned = true; // Indicamos que el jefe ha sido generado
    }

    public void AddWave()
    {
        int previousWave = wave;

        if(score >= 10 && score < 20)
        {
            wave = 2;
        }else if (score >= 20 && score < 30)
        {
            wave = 3;
        }else if (score >= 30 && score < 40)
        {
            wave = 4;
        }
        if(wave != previousWave)
        {
            DestroyRemainingEnemies();
            if(wave < 4)
            {
                waveText.text = "WAVE: " + wave;
            }
        }
    }
    void DestroyRemainingEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    public void OnEnemyBossDeath()
    {
        bossIsSpawned = false;
        wave++;
        
        if (wave >= 5)
        {
            waveText.text = "GANASTE!!!";
        }
    }




}
