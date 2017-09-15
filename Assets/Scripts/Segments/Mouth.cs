using UnityEngine;
using System.Collections;

public class Mouth : Segment {
	private int att_cooldown = 50;
	private int att_cooldown_max = 50;
	public GameObject damage_effect;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (att_cooldown < att_cooldown_max) {
			att_cooldown++;
		}
	}

	void OnCollisionStay2D(Collision2D col){
		if (creature != null && col.collider.gameObject.GetComponent<Segment>()) {
			if (col.collider.gameObject.GetComponent<Segment>().creature != null) {
				if (col.collider.gameObject.tag == "Core") {
					if (col.collider.gameObject.GetComponent<Core> ().creature != creature) {
						if (att_cooldown >= att_cooldown_max) {
							col.collider.gameObject.GetComponent<Core> ().creature.TakeDamage (1);
							mark_damage(col);
							att_cooldown = 0;
						}
					}
				} else if (col.collider.gameObject.tag == "Mouth") {
					if (col.collider.gameObject.GetComponent<Mouth> ().creature != creature) {
						if (att_cooldown >= att_cooldown_max) {
							col.collider.gameObject.GetComponent<Mouth> ().creature.TakeDamage (1);
							mark_damage(col);
							att_cooldown = 0;
						}
					}
				} else if (col.collider.gameObject.tag == "Body") {
					if (col.collider.gameObject.GetComponent<Body> ().creature != creature) {
						if (att_cooldown >= att_cooldown_max) {
							col.collider.gameObject.GetComponent<Body> ().creature.TakeDamage (1);
							mark_damage(col);
							att_cooldown = 0;
						}
					}
				} else if (col.collider.gameObject.tag == "Leg") {
					if (col.collider.gameObject.GetComponent<Leg> ().creature != creature) {
						if (att_cooldown >= att_cooldown_max) {
							col.collider.gameObject.GetComponent<Leg> ().creature.TakeDamage (1);
							mark_damage(col);
							att_cooldown = 0;
						}
					}
				}
			}
			else if (col.collider.gameObject.GetComponent<Segment> ().creature == null) {
				creature.UpdateEvoPoints(creature.evoPoints + (int)Mathf.Floor(col.collider.gameObject.GetComponent<Segment>().pointCost/2));
				Destroy (col.collider.gameObject);
			}
		}
	}
	void mark_damage(Collision2D col){
		GameObject g = (GameObject)Instantiate(damage_effect, 
		                                       col.collider.transform.position, 
		                                       Quaternion.Euler(new Vector3(0,0,transform.rotation.eulerAngles.z+180)));
		g.transform.parent=col.transform;
	}
}
