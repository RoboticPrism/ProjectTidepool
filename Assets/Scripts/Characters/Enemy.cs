using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Creature {
	public GameObject target;
	public bool active=false;

    public Segment corePrefab;
    public List<Segment> bodyPrefabs;
    public List<Segment> mainPrefabs;
    public List<Segment> attackPrefabs;
    public List<Segment> defensePrefabs;
    public List<Segment> movementPrefabs;

    // Use this for initialization
    new void Start () {
		base.Start ();
        evoPoints = level * 100;
        Generate();
	}
	
	// Update is called once per frame
	new void FixedUpdate () {
		base.Update ();
		target = GameObject.FindGameObjectWithTag ("Player");
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("Enemy")) {
			if(g!=this.gameObject){
				if(target==null){
					target=g;
				}
				else{
					if(Vector3.Distance(this.transform.position,g.transform.position)<
			   			Vector3.Distance(this.transform.position,target.transform.position)){
							target=g;
						}
				}
			}
		}
		if (active) {
			speed=tot_speed;
			rot_speed=tot_rot_speed;
			if(target!=null){
			if (this.level<target.GetComponent<Creature>().level) {
				if (target != null) {
					Vector3 targetDir = target.transform.position - transform.position;
					float angle = Mathf.Atan2 (targetDir.y, targetDir.x) * Mathf.Rad2Deg;
					Quaternion q = Quaternion.AngleAxis (angle + 90, Vector3.forward);
					transform.rotation = Quaternion.Slerp (transform.rotation, q, Time.deltaTime * rot_speed/10);
					float offset = Vector3.Distance(target.transform.position, transform.position);
					if(offset > 50){
						speed = 0;
					}
					else{
						speed = tot_speed;
					}
					transform.Translate(Vector3.up*speed/100);
				}

				//GetComponent<Rigidbody2D> ().AddForce (this.transform.up * new Vector3 (1, 0, 0));
			} else {
				if (target != null) {
					Vector3 targetDir = target.transform.position - transform.position;
					float angle = Mathf.Atan2 (targetDir.y, targetDir.x) * Mathf.Rad2Deg;
					Quaternion q = Quaternion.AngleAxis (angle - 90, Vector3.forward);
					transform.rotation = Quaternion.Slerp (transform.rotation, q, Time.deltaTime * rot_speed/10);
					float offrot = Mathf.Abs(angle-90-transform.rotation.eulerAngles.z);
					if(offrot>180){
						offrot=360-offrot;
					}
					//Debug.Log (offrot);
					if(offrot > 90){
						speed = 0;
					}
					else{
						speed = tot_speed*(1-offrot/90);
					}
					transform.Translate(Vector3.up*speed/100);
				}

			}
			}
		} else {
			speed=0;
			rot_speed=0;
			GetComponent<Rigidbody2D>().velocity=new Vector2(0,0);
		}
		if (evoPoints > 30 && level == 1) {
			Upgrade();
		}
		if (evoPoints > 100 && level == 2) {
			Upgrade();
		}
	}

    // randomly builds a new enemy layout
    void Generate()
    {
        GenerateBodyParts(100);
        GenerateMainParts(100);
    }

    // generates a copy of an existing enemy layout
    void GenerateExisting(Enemy enemy)
    {

    }

    // generates the building block body segments procedurally
    void GenerateBodyParts(int evoPointAllowance)
    {
        
    }

    // generates the vital parts like mouths procedurally
    void GenerateMainParts(int evoPointAllowance)
    {

    }

    // generates the attack segments procedurally
    void GenerateAttackParts(int evoPointAllowance)
    {

    }

    // generates the defense segments procedurally
    void GenerateDefenseParts(int evoPointAllowance)
    {

    }

    // generates the movement segments procedurally
    void GenerateMovementParts(int evoPointAllowance)
    {

    }

    void AddPartYSymetrical(Vector2 buildUnits, int rot, Segment segment)
    {
        if (buildUnits.x == 0)
        {
            AddSegment(buildUnits, rot, segment);
        }
        else
        {
            AddSegment(buildUnits, rot, segment);
            int _altRot = rot;
            if (rot == 90)
            {
                _altRot = 270;
            } else if (rot == 270)
            {
                _altRot = 90;
            }
            AddSegment(buildUnits, _altRot, segment);
        }
    }

	void Upgrade(){
		for (int x =0; x<height*2+1; x++) {
			for (int y =0; y<width*2+1; y++){
				if(segments[x,y]!=null){
					Destroy (segments[x,y]);
					segments[x,y]=null;
				}
			}
		}
		if (level < 3) {
			level++;
			if(level==2){
				playerColor = new Color32(239,242,21,255);
			}
			else if(level==3){
				playerColor = new Color32(242,62,21,255);
			}
			this.Start();
			active=true;
		}

	}
}
