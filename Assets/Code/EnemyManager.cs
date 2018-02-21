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

    // Use this for initialization
    void Start ()
    {
        t_lastSpawned = DateTime.MinValue;
        spawnEvery = TimeSpan.FromSeconds(SpawnEverySeconds);
    }
	
	// Update is called once per frame
	void Update ()
    {
        var now = DateTime.Now;
        if (enemiesSpawned < MaxEnemy && now - t_lastSpawned > spawnEvery)
        {
            int i = UnityEngine.Random.Range(0, 5);
            var enemy = UnityEngine.Object.Instantiate(EnemyPrefab, SpawnPoints[i].transform.position, Quaternion.identity);
            var enemyComp = enemy.GetComponent<Enemy>();
            enemyComp.aPlayer = aPlayer;
            enemyComp.OnDead += onDie;

            ++enemiesCounter;
            ++enemiesSpawned;
            t_lastSpawned = now;
        }
	}

    void onDie(object sender, Enemy.EnemyInfo i)
    {
        var comp = sender as Enemy;
        if (comp != null)
        {
            --enemiesSpawned;
            var collider = comp.gameObject.GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            UnityEngine.Object.Destroy(comp.gameObject, 3.0f);
        }
    }
}
