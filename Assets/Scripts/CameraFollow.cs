using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Player player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (player != null) {
			transform.position = new Vector3 (player.transform.position.x,
		                                  player.transform.position.y,
		                                  transform.position.z);
			transform.rotation = player.transform.rotation;
		}
	}
}
