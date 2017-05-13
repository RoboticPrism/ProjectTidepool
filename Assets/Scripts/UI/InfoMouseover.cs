using UnityEngine;
using System.Collections;

public class InfoMouseover : MonoBehaviour {

	public GameObject info;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnMouseOver() {
		info.SetActive (true);
	}

	public void OnMouseExit() {
		info.SetActive (false);
	}
}
