using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreEye : Segment {

    GameObject target;

	// Use this for initialization
	void Start () {
        creature = transform.parent.GetComponent<Segment>().creature;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    new void FixedUpdate ()
    {
        base.FixedUpdate();
        if (creature)
        {
            if (creature.GetComponent<Player>())
            {
                foreach (GameObject g in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    if (g != this.gameObject)
                    {
                        if (target == null)
                        {
                            target = g;
                        }
                        else
                        {
                            if (Vector3.Distance(this.transform.position, g.transform.position) <
                                   Vector3.Distance(this.transform.position, target.transform.position))
                            {
                                target = g;
                            }
                        }
                    }
                }
            }
            else if (creature.GetComponent<Enemy>())
            {
                target = creature.GetComponent<Enemy>().target;
            }
        }
        if (target)
        {
            if (creature.GetComponent<Player>() && creature.GetComponent<Player>().build)
            {
                transform.rotation = Quaternion.Euler(Vector3.forward);
            }
            else
            {
                Vector3 targetDir = target.transform.position - transform.position;
                float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
                Quaternion q = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 1);
            }
        }
        //transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * totalRotationSpeed / 10);
    }
}
