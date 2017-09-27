﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBit : Segment {
    private int attCooldown = 100;
    private int attCooldownMax = 100;
    public GameObject damageEffect;
    // Use this for initialization
    void Start () {
        creature = transform.parent.GetComponent<Segment>().creature;
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
        if (attCooldown < attCooldownMax)
        {
            attCooldown++;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
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
    }

    void MarkDamage(Collision2D col)
    {
        GameObject g = (GameObject)Instantiate(damageEffect,
                                               col.collider.transform.position,
                                               Quaternion.Euler(new Vector3(0, 0, transform.rotation.eulerAngles.z + 180)));
        g.transform.parent = col.transform;
    }
}
