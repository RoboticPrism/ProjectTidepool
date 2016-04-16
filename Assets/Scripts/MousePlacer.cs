using UnityEngine;
using System.Collections;

public class MousePlacer : MonoBehaviour {
	public Player player;
	public Sprite mouth;
	public Sprite body;
	public Sprite spike;
	public Sprite leg;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Vector3 mouse=Camera.main.ScreenToWorldPoint(Input.mousePosition)-player.transform.position;
		this.transform.localPosition=new Vector3(Mathf.RoundToInt(mouse.x)
		                                         ,Mathf.RoundToInt(mouse.y),0);
		this.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, player.rot));
		if (player.build && Mathf.RoundToInt(mouse.x)<player.width+1
		    			 && Mathf.RoundToInt(mouse.x)>-player.width-1
		    			 &&Mathf.RoundToInt(mouse.y)<player.height+1
		    			 &&Mathf.RoundToInt(mouse.y)>-player.height-1) {
			/*
			Vector3 obj = Camera.main.WorldToScreenPoint (player.transform.position);
			this.transform.position = new Vector3 ((Mathf.RoundToInt ((Input.mousePosition.x) / 32)-obj.x+player.transform.position.x), 
			                                       (Mathf.RoundToInt ((Input.mousePosition.y) / 32)-obj.y+player.transform.position.y),
			                                       0);
			*/
			if (player.type == 1 && player.number_mouth>0) {
				if(GetComponent<SpriteRenderer> ().sprite != mouth && player.number_mouth>0){
					GetComponent<SpriteRenderer> ().sprite = mouth;
				}
			} else if (player.type == 2 && player.number_body>0) {
				if(GetComponent<SpriteRenderer> ().sprite != body){
					GetComponent<SpriteRenderer> ().sprite = body;
				}
			} else if (player.type == 3 && player.number_spike>0) {
				if(GetComponent<SpriteRenderer> ().sprite != spike){
					GetComponent<SpriteRenderer> ().sprite = spike;
				}
			} else if (player.type == 4 && player.number_leg>0) {
				if(GetComponent<SpriteRenderer> ().sprite != leg){
					GetComponent<SpriteRenderer> ().sprite = leg;
				}
			} else {
				GetComponent<SpriteRenderer> ().sprite = null;
			}
		} else {
			GetComponent<SpriteRenderer> ().sprite = null;
		}
	}
}
