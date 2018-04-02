using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {
	public GameObject target;
	public GameObject enemyPrefab;
	public int rangeMin=30;
	public int rangeMax=60;
	public int maxEnemies = 15;
	Vector3 loc = new Vector3(0,0,0);
    List<Enemy> enemyList = new List<Enemy>();
    public List<Enemy>[] designList = new List<Enemy>[6];
	// Use this for initialization
	void Start () {
	    for (int i = 0; i < designList.Length; i++)
        {
            designList[i] = new List<Enemy>();
        }
	}
	
	// Update is called once per frame
	void Update () {
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
	}

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
            // add to list
            designList[newEnemy.level].Add(newDesignEnemy);
        }
    }

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
    }
}
