﻿using UnityEngine;
using System.Collections;

public class Enemy : Creature {
	public GameObject target;
	public bool active=false;

    public Segment corePrefab;
    public Segment bodyPrefab;
    public Segment mouthPrefab;
    public Segment legsPrefab;
    public Segment spikePrefab;
    // Use this for initialization
    new void Start () {
		base.Start ();
		int r = Random.Range (0, 3);
		if (level == 1) {
			if (r == 0) {
				Config1 ();
				health = 6;
				tot_speed = 3;
				tot_rot_speed = 4;
			} else if (r == 1) {
				Config2 ();
				health = 6;
				tot_speed = 2;
				tot_rot_speed = 1;
			} else {
				Config3();
				health = 8;
				tot_speed = 2;
				tot_rot_speed = 1;
			}
		}
		else if (level == 2) {
			if (r == 0) {
				Config4 ();
				health = 10;
				tot_speed = 3;
				tot_rot_speed = 4;
			} else if (r == 1) {
				Config5 ();
				health = 8;
				tot_speed = 2;
				tot_rot_speed = 1;
			} else {
				Config6();
				health = 12;
				tot_speed = 2;
				tot_rot_speed = 1;
			}
		}
		else if (level == 3) {
			if (r == 0) {
				Config7 ();
				health = 6;
				tot_speed = 3;
				tot_rot_speed = 4;
			} else if (r == 1) {
				Config8 ();
				health = 6;
				tot_speed = 2;
				tot_rot_speed = 1;
			} else {
				Config9();
				health = 8;
				tot_speed = 2;
				tot_rot_speed = 1;
			}
		}

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
		if (evo_points > 30 && level == 1) {
			Upgrade();
		}
		if (evo_points > 100 && level == 2) {
			Upgrade();
		}
	}

	void AddPart(int x, int y, int rot, Segment segment){
		if (segment.multidirectional) {
			segments[max_height+y,max_width+x] = (GameObject)Instantiate(segment.gameObject, 
			                                                   this.transform.position+new Vector3(x,y,0), 
			                                                   Quaternion.Euler(new Vector3(0,0,0)));
		} else {
			segments[max_height + y, max_width + x] = (GameObject)Instantiate(segment.gameObject, 
			                                                     this.transform.position+new Vector3(x,y,0), 
			                                                     Quaternion.Euler(new Vector3(0,0,rot)));
		}
		segments [max_height + y, max_width + x].transform.parent = transform;
		segments [max_height + y, max_width + x].GetComponent<Segment>().creature = this;
		if (segment.useColor) {
			segments [max_height + y, max_width + x].GetComponent<SpriteRenderer> ().color = playerColor;
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

	void Config1(){
		AddPart (1, 0, 0, corePrefab);
	}
	void Config2(){
        AddPart(1, 0, 0, corePrefab);
    }
	void Config3(){
        AddPart(1, 0, 0, corePrefab);
    }
	//level 2 configs
	void Config4(){
        AddPart(1, 0, 0, corePrefab);
    }
	void Config5(){
        AddPart(1, 0, 0, corePrefab);
    }
	void Config6(){
        AddPart(1, 0, 0, corePrefab);
    }
	void Config7(){
        AddPart(1, 0, 0, corePrefab);

    }
	void Config8(){
        AddPart(1, 0, 0, corePrefab);
    }
	void Config9(){
        AddPart(1, 0, 0, corePrefab);
    }
}
