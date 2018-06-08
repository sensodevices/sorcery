using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyManager : MonoBehaviour {

    public Transform[] SpawnPoints;
    public Transform EnemyPrefab;
    public float SpawnEverySeconds = 15.0f;
    public int MaxEnemy = 3;
    public Player aPlayer;

    private TimeSpan spawnEvery;
    private int enemiesSpawned = 0;
    private int enemiesCounter = 0;
    private DateTime t_lastSpawned;

    private Enemy[] spawnedEnemies;

    // Use this for initialization
    void Start ()
    {
        t_lastSpawned = DateTime.MinValue;
        spawnEvery = TimeSpan.FromSeconds(SpawnEverySeconds);
        spawnedEnemies = new Enemy[MaxEnemy];
        var spawnPoint = new Vector3(0.0f, -999.0f, 0.0f);
        for (int i = 0; i < MaxEnemy; ++i)
        {
            var obj = UnityEngine.Object.Instantiate(EnemyPrefab, spawnPoint, Quaternion.identity);
            var enemyComp = obj.GetComponent<Enemy>();
            enemyComp.aPlayer = aPlayer;
            enemyComp.OnDead += onDie;
            spawnedEnemies[i] = enemyComp;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        var now = DateTime.Now;
        if (enemiesSpawned < MaxEnemy && now - t_lastSpawned > spawnEvery)
        { // Spawn enemy
            int spawnPointInd = UnityEngine.Random.Range(0, 5);
            var ind = getFirstUnusedEnemyInd();
            if (ind >= 0) { 
                spawnedEnemies[ind].Spawn(SpawnPoints[spawnPointInd].position);
                ++enemiesCounter;
                ++enemiesSpawned;
                t_lastSpawned = now;
            }
        }
	}

    void onDie(object sender, Enemy.EnemyInfo i)
    {
        var comp = sender as Enemy;
        if (comp != null)
        {
            --enemiesSpawned;
        }
    }

    private int getFirstUnusedEnemyInd()
    {
        for (int i = 0; i < MaxEnemy; ++i)
        {
            if (spawnedEnemies[i].IsDead) return i;
        }
        return -1;
    }
}
