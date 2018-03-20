using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {
	public GameObject target;
	public GameObject enemy;
	public int rangeMin=30;
	public int rangeMax=60;
	public int maxEnemies = 15;
	Vector3 loc = new Vector3(0,0,0);
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (GameObject.FindGameObjectsWithTag ("Enemy").Length < maxEnemies) {
			CreateEnemy();
		}
		if (target != null) {
			loc = target.transform.position;
		}
	}

    // creates an enemy of a random level in a random place
	void CreateEnemy(){
		int radius = Random.Range (0, 360);
		int distance = Random.Range (rangeMin, rangeMax);
		int power = Random.Range (0, 5);
		bool act = true;
		if (target != null && target.GetComponent<Player>()) {
			act = !target.GetComponent<Player>().build;
		} else {
			act=true;
		}
		GameObject g = Instantiate(
                enemy, 
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
	}
}
