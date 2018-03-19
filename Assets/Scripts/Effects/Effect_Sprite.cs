using UnityEngine;
using System.Collections;

public class Effect_Sprite : MonoBehaviour {

	int life =30;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (life > 0) {
			life--;
		} else {
			Destroy(gameObject);
		}
	}
}
