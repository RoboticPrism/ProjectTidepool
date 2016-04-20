using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : Creature {

	public GameObject ValidSpace;
	public GameObject BuildGrid;
	public GameObject canvas;
	public GameObject mousePlacer;
	public GameObject EvoPoints;
	public GameObject MouthCount;
	public GameObject BodyCount;
	public GameObject SpikeCount;
	public GameObject LegCount;
	public GameObject BuildWarning;
	private int size = 0;
	public int type = 1;
	public int rot = 0;
	public bool build = true;
	bool canbuild =true;
	private int warningTimer=0;



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
		mousePlacer.GetComponent<SpriteRenderer> ().color = playerColor;
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
			AddSegment ((Mathf.RoundToInt (mouse.x)), 
			            (Mathf.RoundToInt (mouse.y)),
			            rot,
			            type);
			Redraw ();
		} 
		else if (Input.GetMouseButtonDown (1) && build) {
			Vector3 obj = Camera.main.WorldToScreenPoint (this.transform.position);
			RemoveSegment ((Mathf.RoundToInt ((Input.mousePosition.x - obj.x) / size)), 
			               (Mathf.RoundToInt ((Input.mousePosition.y - obj.y) / size)));
			Redraw ();
		}

		if (Input.GetButtonDown("ToggleBuild")) {
			SetMode ();
		}

		if (Input.GetButtonDown ("Rotate")) {
			rotate();
		}
		if (Input.GetButtonDown ("Swap")) {
			type++;
			if(type>3){
				type=1;
			}
			rot = 0;
		}
		EvoPoints.GetComponent<Text> ().text = evo_points.ToString();
		MouthCount.GetComponent<Text>().text = "-20";
		BodyCount.GetComponent<Text>().text = "-10";
		SpikeCount.GetComponent<Text>().text = "-12";
		LegCount.GetComponent<Text>().text = "-10";
	}

	void FixedUpdate(){
		removeText ();
	}



	void CreateBuildObjects(){
		Instantiate(BuildGrid,this.transform.position,Quaternion.Euler(new Vector3(0,0,transform.rotation.eulerAngles.z)));
		for(int i = 0; i < width*2+1; i++){
			for(int j = 0; j < height*2+1; j++){
				//Debug.Log (""+i+" "+j+" "+placeable[i,j]);
				if(placeable[i,j]){
					GameObject vs = (GameObject)Instantiate(ValidSpace, new Vector3(0,0,0), Quaternion.Euler(new Vector3(0,0,transform.rotation.eulerAngles.z)));
					vs.transform.parent=this.transform;
					vs.transform.localPosition=new Vector3(i-width//+this.transform.localPosition.x
					                                  ,j-height//+this.transform.localPosition.y
					                                  ,0);
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
			canvas.SetActive(false);
			foreach(GameObject g in GameObject.FindGameObjectsWithTag("Enemy")){
				g.GetComponent<Enemy>().active=true;
			}
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
				canvas.SetActive(true);
				foreach(GameObject g in GameObject.FindGameObjectsWithTag("Enemy")){
					g.GetComponent<Enemy>().active=false;
				}
			}
			else{
				BuildWarning.SetActive(true);
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
	public void setMouth(){
		type = 1;
	}
	public void setBody(){
		type = 2;
	}
	public void setSpike(){
		type = 3;
	}
	public void setLeg(){
		type = 4;
	}
	public void rotate(){
		rot-=90;
		if(rot==-90){
			rot=270;
		}
	}
	public void removeText(){
		if (warningTimer > 0) {
			BuildWarning.GetComponent<Text>().color=new Color(255,255,255,warningTimer);
			warningTimer-=3;

		} else {
			BuildWarning.SetActive(false);
		}
	}
	
}
