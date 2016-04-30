using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : Creature {

	public GameController gameController;
	private int size = 0;
	public int type = 0;
	public int rot = 0;
	public bool build = true;
	bool canbuild =true;
	private int warningTimer=0;
	public int bodyPrice = 10;
	public int mouthPrice = 20;
	public int legPrice = 8;
	public int spikePrice = 12;

	// Use this for initialization
	void Start () {
		base.Start ();
		//number_body = 15;
		//number_spike = 15;
		//number_leg = 15;
		size = 32;
		Redraw ();
		tot_speed = 1;
		tot_rot_speed = 1;
		tot_energy = 1;
	}

	// Update is called once per frame
	void Update () {
		base.Update ();
		if (!build) {
			rot_speed += Input.GetAxis ("Horizontal");
			speed += Input.GetAxis ("Vertical");
			if (speed > tot_speed) {
				speed = tot_speed;
			}
			if (speed < -tot_speed) {
				speed = -tot_speed;
			}
			if (rot_speed > tot_rot_speed) {
				rot_speed = tot_rot_speed;
			}
			if (rot_speed < -tot_rot_speed) {
				rot_speed = -tot_rot_speed;
			}

			GetComponent<Rigidbody2D> ().velocity = transform.up*speed;

			transform.rotation=Quaternion.Euler(0,0, transform.rotation.eulerAngles.z-rot_speed/10);

			if (Input.GetAxis ("Vertical") == 0) {
				speed *= .5f;
			}
			if (Input.GetAxis ("Horizontal") == 0) {
				rot_speed *= .5f;
			}
		} 
		else {
			GetComponent<Rigidbody2D>().velocity = new Vector2 (0,0);
			GetComponent<Rigidbody2D>().angularVelocity = 0;
		}
		if (Input.GetMouseButtonDown (0) && build) {
			//objects point on the screen
			Vector3 mouse=Camera.main.ScreenToWorldPoint(Input.mousePosition)-transform.position;
			float x = mouse.x;
			float y = mouse.y;
			float temp_rot = Mathf.PI * transform.rotation.eulerAngles.z / 180;
			AddSegment (
				Mathf.RoundToInt (x * Mathf.Cos(temp_rot) + y * Mathf.Sin(temp_rot)), 
				Mathf.RoundToInt (-x * Mathf.Sin(temp_rot) + y * Mathf.Cos(temp_rot)),
				rot,
				type);
			Redraw ();
		} 
		else if (Input.GetMouseButtonDown (1) && build) {
			Vector3 mouse = Camera.main.WorldToScreenPoint (this.transform.position);
			float x = mouse.x;
			float y = mouse.y;
			float temp_rot = Mathf.PI * transform.rotation.eulerAngles.z / 180;
			RemoveSegment (
				Mathf.RoundToInt (x * Mathf.Cos(temp_rot) + y * Mathf.Sin(temp_rot)), 
				Mathf.RoundToInt (-x * Mathf.Sin(temp_rot) + y * Mathf.Cos(temp_rot))
			);
			Redraw ();
		}

		if (Input.GetButtonDown("ToggleBuild")) {
			SetMode ();
		}
	}

	void FixedUpdate(){
		base.FixedUpdate ();
		removeText ();
	}



	void CreateBuildObjects(){
		for(int i = 0; i < width*2+1; i++){
			for(int j = 0; j < height*2+1; j++){
				//Debug.Log (""+i+" "+j+" "+placeable[i,j]);
				if (placeable [i, j]) {
					GameObject vs = (GameObject)Instantiate (gameController.placeable, new Vector3 (0, 0, 0), Quaternion.Euler (new Vector3 (0, 0, transform.rotation.eulerAngles.z)));
					vs.transform.parent = this.transform;
					vs.transform.localPosition = new Vector3 (i - width//+this.transform.localPosition.x
						, j - height//+this.transform.localPosition.y
						, 0);
				} else {
					GameObject vs = (GameObject)Instantiate (gameController.gridCell, new Vector3 (0, 0, 0), Quaternion.Euler (new Vector3 (0, 0, transform.rotation.eulerAngles.z)));
					vs.transform.parent = this.transform;
					vs.transform.localPosition = new Vector3 (i - width//+this.transform.localPosition.x
						, j - height//+this.transform.localPosition.y
						, 0);
				}
			}
		}
	}

	void SetMode(){
		//turn building mode off
		if (build) {
			foreach(GameObject g in GameObject.FindGameObjectsWithTag("BuildMode")){
				Destroy(g);
			}
			build=false;
			gameController.buildCanvas.gameObject.SetActive(false);
			gameController.playCanvas.gameObject.SetActive(true);
			foreach(GameObject g in GameObject.FindGameObjectsWithTag("Enemy")){
				g.GetComponent<Enemy>().active=true;
			}
			GetComponent<Rigidbody2D> ().mass = weight;
		} 
		//turn building mode on
		else {
			canbuild=true;
			foreach(GameObject g in GameObject.FindGameObjectsWithTag("Enemy")){
				if(Vector3.Distance(g.transform.position,this.transform.position)<15){
					canbuild=false;
				}
			}
			if(canbuild){
				CreateBuildObjects();
				build=true;
				gameController.buildCanvas.gameObject.SetActive(true);
				gameController.playCanvas.gameObject.SetActive(false);
				foreach(GameObject g in GameObject.FindGameObjectsWithTag("Enemy")){
					g.GetComponent<Enemy>().active=false;
				}
			}
			else{
				gameController.buildWarning.gameObject.SetActive(true);
				warningTimer=255;
			}

		}
	}

	void Redraw(){
		foreach(GameObject g in GameObject.FindGameObjectsWithTag("BuildMode")){
			Destroy(g);
		}
		CreateBuildObjects();
	}
	public void removeText(){
		if (warningTimer > 0) {
			gameController.buildWarning.GetComponent<Text>().color=new Color(255,255,255,warningTimer);
			warningTimer-=3;

		} else {
			gameController.buildWarning.gameObject.SetActive(false);
		}
	}

}
