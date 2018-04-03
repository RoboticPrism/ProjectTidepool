using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {
    [Header("External Connections")]
    [Tooltip("The target in which enemies are spawned around")]
    public GameObject target;

    [Header("Prefabs")]
    [Tooltip("The enemy prefab to instantiate new enemies off of")]
    public GameObject enemyPrefab;

    [Header("Spawn Settings")]
    [Tooltip("The minimum range from target the spawner will spawn a new enemy in")]
    public int rangeMin = 30;
    [Tooltip("The maximum range from target the spawner will spawn a new enemy in")]
    public int rangeMax = 60;
    [Tooltip("The maximum amount of enemies the spawner will have out at once")]
    public int maxEnemies = 15;
    [Tooltip("The number of designs that can be stored at once")]
    public int maxDesignPoolSize = 5;

    Vector3 loc = new Vector3(0, 0, 0);
    List<Enemy> enemyList = new List<Enemy>();
    List<Enemy>[] designList = new List<Enemy>[6];

    void Start() {
        for (int i = 0; i < designList.Length; i++)
        {
            designList[i] = new List<Enemy>();
            GameObject g = new GameObject();
            g.transform.parent = this.transform;
            g.name = "Level " + (i + 1) + " Designs";
        }
    }

    void FixedUpdate() {
        enemyList.RemoveAll(enemy => enemy == null);
        if (enemyList.Count < maxEnemies) {
            CreateEnemy();
        }
        // if any enemies are too far away, delete them
        foreach (Enemy enemy in enemyList)
        {
            if (Vector3.Distance(loc, enemy.transform.position) > rangeMax * 2) {
                Destroy(enemy.gameObject);
            }
        }
        if (target) {
            loc = target.transform.position;
        }
        ReturnEnemyToPlayArea();
    }

    // if an enemy wanders too far out of bounds, bounce them back into the fray
    void ReturnEnemyToPlayArea()
    {
        foreach(Enemy enemy in enemyList)
        {
            if (Vector3.Distance(enemy.transform.position, target.transform.position) > rangeMax * 2)
            {
                int radius = Random.Range(0, 360);
                int distance = Random.Range(rangeMin, rangeMax);
                enemy.transform.position = new Vector3(
                    loc.x + Mathf.Cos(radius) * distance,
                    loc.y + Mathf.Sin(radius) * distance,
                    0
                );
            }
        }
    }

    // registers a copy of this enemy as a design
    public void RegisterDesign(Enemy newEnemy)
    {
        if (newEnemy.level > 0 && newEnemy.level < designList.Length) {
            // make an invisible copy to store under the spawner
            GameObject newDesign = Instantiate(newEnemy.gameObject, transform);
            newDesign.SetActive(false);

            // reset stats to spawn defaults
            Enemy newDesignEnemy = newDesign.GetComponent<Enemy>();
            newDesignEnemy.health = newDesignEnemy.totalHealth;
            newDesignEnemy.evoPoints = 0;
            newDesignEnemy.target = null;

            // reset egg progress
            Destroy(newDesignEnemy.eggObject);

            // parent it to a subobject for organization
            newDesignEnemy.transform.parent = transform.GetChild(newDesignEnemy.level - 1);

            // add to list
            designList[newEnemy.level - 1].Add(newDesignEnemy);

            // keep pool within max size
            if(designList[newEnemy.level - 1].Count > maxDesignPoolSize)
            {
                designList[newEnemy.level - 1].RemoveAt(designList[newEnemy.level - 1].Count - 1);
            }
        }
    }

    // creates a new enemy either randomly or from the pool
    void CreateEnemy()
    {
        int power = Random.Range(0, 5);
        if (designList[power] != null && designList[power].Count > 0)
        {
            CreateDesignedEnemy(power);
        } else
        {
            CreateRandomEnemy(power);
        }
    }

    // creates a new random enemy of a random level in a random place
	void CreateRandomEnemy(int power){
		int radius = Random.Range (0, 360);
		int distance = Random.Range (rangeMin, rangeMax);
		bool act = true;
		if (target != null && target.GetComponent<Player>()) {
			act = !target.GetComponent<Player>().build;
		} else {
			act=true;
		}
		GameObject g = Instantiate(
                enemyPrefab, 
                new Vector3(
                    loc.x + Mathf.Cos(radius) * distance,
                    loc.y + Mathf.Sin(radius) * distance, 
                    0
                ),
                Quaternion.Euler(new Vector3(0, 0, 0)));
        Enemy newEnemy = g.GetComponent<Enemy>();
        newEnemy.active = act;
        newEnemy.level = power;
        newEnemy.evoPoints = 50 * (power + 2);
        newEnemy.spawner = this;
        enemyList.Add(newEnemy);
	}

    // creates a copy of an existing winning design
    void CreateDesignedEnemy(int power)
    {
        int radius = Random.Range(0, 360);
        int distance = Random.Range(rangeMin, rangeMax);
        bool act = true;
        if (target != null && target.GetComponent<Player>())
        {
            act = !target.GetComponent<Player>().build;
        }
        else
        {
            act = true;
        }
        GameObject g = Instantiate(
                designList[power][Random.Range(0, designList[power].Count)].gameObject,
                new Vector3(
                    loc.x + Mathf.Cos(radius) * distance,
                    loc.y + Mathf.Sin(radius) * distance,
                    0
                ),
                Quaternion.Euler(new Vector3(0, 0, 0)));
        g.SetActive(true);
        Enemy newEnemy = g.GetComponent<Enemy>();
        newEnemy.active = act;
        newEnemy.spawner = this;
        enemyList.Add(newEnemy);
    }
}
