using UnityEngine;
using System.Collections;

public class Mouth : Segment {
	private int attCooldown = 50;
	private int attCooldownMax = 50;
	public GameObject damageEffect;
	// Use this for initialization
	void Start ()
    {
        
	}
	
	new void FixedUpdate ()
    {
        base.FixedUpdate();
		if (attCooldown < attCooldownMax)
        {
			attCooldown++;
		}
	}

	void OnCollisionStay2D(Collision2D col)
    {
        Segment collidedSegment = col.collider.gameObject.GetComponent<Segment>();
        if (creature != null && collidedSegment && collidedSegment.GetComponent<SpikeBit>() == null)
        {
            if (collidedSegment.creature && collidedSegment.creature != creature)
            {
                if (attCooldown >= attCooldownMax)
                {
                    collidedSegment.creature.TakeDamage(1);
                    MarkDamage(col);
                    attCooldown = 0;
                }
            }
        }
        else if (creature && collidedSegment.creature == null && collidedSegment.edible)
        {
            creature.UpdateEvoPoints(creature.evoPoints + (int)Mathf.Floor(collidedSegment.pointCost / 2));
            Destroy(col.collider.gameObject);
        }
	}
	void MarkDamage(Collision2D col){
		GameObject g = (GameObject)Instantiate(damageEffect, 
		                                       col.collider.transform.position, 
		                                       Quaternion.Euler(new Vector3(0,0,transform.rotation.eulerAngles.z+180)));
		g.transform.parent=col.transform;
	}
}
