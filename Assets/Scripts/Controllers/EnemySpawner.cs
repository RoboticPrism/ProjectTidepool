﻿using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {
	public GameObject player;
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
		if (player != null) {
			loc=player.transform.position;
		}
	}

    // creates an enemy of a random level in a random place
	void CreateEnemy(){
		int r = Random.Range (0, 360);
		int d = Random.Range (rangeMin, rangeMax);
		int p = Random.Range (0, 10); // this needs some serious love, this whole system is massively illegible
		bool act = true;
		if (player != null) {
			act = !player.GetComponent<Player> ().build;
		} else {
			act=true;
		}
		GameObject g;
        if (p > 8)
        {
            g = (GameObject)Instantiate(enemyL3, new Vector3(loc.x + Mathf.Cos(r) * d,
                                               loc.y + Mathf.Sin(r) * d, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
        }
        else if (p > 5)
        {
			g = (GameObject)Instantiate (enemyL2, new Vector3 (loc.x + Mathf.Cos (r) * d, 
		                                 	   loc.y + Mathf.Sin (r) * d, 0), Quaternion.Euler (new Vector3 (0, 0, 0)));
		}
        else
        {
			g = (GameObject)Instantiate (enemyL1, new Vector3 (loc.x + Mathf.Cos (r) * d, 
			                                   loc.y + Mathf.Sin (r) * d, 0), Quaternion.Euler (new Vector3 (0, 0, 0)));
		}
		g.GetComponent<Enemy> ().active = act;
	}
}