using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public Player player;
	public bool build = false;
	public int Body_Segments=4;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Space")) {
			build=!build;
		}
	}
}
