using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {
	public GameObject target;
	public GameObject enemyL1;
	public GameObject enemyL2;
	public GameObject enemyL3;
	public GameObject enemyL4;
	public GameObject enemyL5;
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
		int power = Random.Range (0, 10); // this needs some serious love, this whole system is massively illegible
		bool act = true;
		if (target != null && target.GetComponent<Player>()) {
			act = !target.GetComponent<Player>().build;
		} else {
			act=true;
		}
		GameObject g;
        if (power > 8)
        {
            g = (GameObject)Instantiate(
                enemyL3, 
                new Vector3(
                    loc.x + Mathf.Cos(radius) * distance,
                    loc.y + Mathf.Sin(radius) * distance, 
                    0
                ),
                Quaternion.Euler(new Vector3(0, 0, 0)));
        }
        else if (power > 5)
        {
			g = (GameObject)Instantiate (
                enemyL2,
                new Vector3(
                    loc.x + Mathf.Cos(radius) * distance,
                    loc.y + Mathf.Sin(radius) * distance,
                    0
                ),
                Quaternion.Euler(new Vector3(0, 0, 0)));
        }
        else
        {
			g = (GameObject)Instantiate (
                enemyL1,
                new Vector3(
                    loc.x + Mathf.Cos(radius) * distance,
                    loc.y + Mathf.Sin(radius) * distance,
                    0
                ),
                Quaternion.Euler(new Vector3(0, 0, 0)));
        }
		g.GetComponent<Enemy> ().active = act;
	}
}
