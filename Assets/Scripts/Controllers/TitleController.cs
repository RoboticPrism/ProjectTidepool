using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GotoPlay(){
		SceneManager.LoadScene (1);
	}
}
