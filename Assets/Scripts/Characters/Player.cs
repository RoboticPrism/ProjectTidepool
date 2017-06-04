using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : Creature {

	public GameController gameController;
	private int size = 0;
	public Segment selectedSegment;
	public int rot = 0;
	public bool build = true;
	bool canbuild =true;
	private int warningTimer=0;
    public Text evoPointsText;

	// Use this for initialization
	new void Start () {
		base.Start ();
		size = 32;
		Redraw ();
		tot_speed = 1;
		tot_rot_speed = 1;
		tot_energy = 1;
        UpdateEvoPoints(evoPoints);
	}

	// Update is called once per frame
	new void Update () {
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
				new Vector2(
                    Mathf.RoundToInt (x * Mathf.Cos(temp_rot) + y * Mathf.Sin(temp_rot)), 
                    Mathf.RoundToInt (-x * Mathf.Sin(temp_rot) + y * Mathf.Cos(temp_rot))),
				rot,
				selectedSegment);
			Redraw ();
		} 
		else if (Input.GetMouseButtonDown (1) && build) {
			Vector3 mouse=Camera.main.ScreenToWorldPoint(Input.mousePosition)-transform.position;
			float x = mouse.x;
			float y = mouse.y;
			float temp_rot = Mathf.PI * transform.rotation.eulerAngles.z / 180;
			RemoveSegment (
				new Vector2(
                    Mathf.RoundToInt (x * Mathf.Cos(temp_rot) + y * Mathf.Sin(temp_rot)), 
				    Mathf.RoundToInt (-x * Mathf.Sin(temp_rot) + y * Mathf.Cos(temp_rot)))
			);
			Redraw ();
		}

		if (Input.GetButtonDown("ToggleBuild")) {
			SetMode ();
		}
	}

	new void FixedUpdate(){
		base.FixedUpdate ();
		removeText ();
	}

    public override void UpdateEvoPoints(int newEvoPoints)
    {
        base.UpdateEvoPoints(newEvoPoints);
        evoPointsText.text = newEvoPoints.ToString();
    }

	void CreateBuildObjects(){
		for(int i = 0; i < placeable.GetLength(0); i++){
			for(int j = 0; j < placeable.GetLength(1); j++){
                Vector2 buildUnits = ArrayToBuildUnits(new Vector2(i, j));
                if (BoundsCheck(buildUnits))
                {
                    if (GetPlaceable(buildUnits))
                    {
                        GameObject vs = (GameObject)Instantiate(gameController.placeable, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, transform.rotation.eulerAngles.z)));
                        vs.transform.parent = this.transform;
                        vs.transform.localPosition = buildUnits;
                    }
                    else
                    {
                        GameObject vs = (GameObject)Instantiate(gameController.gridCell, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, transform.rotation.eulerAngles.z)));
                        vs.transform.parent = this.transform;
                        vs.transform.localPosition = buildUnits;
                    }
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
