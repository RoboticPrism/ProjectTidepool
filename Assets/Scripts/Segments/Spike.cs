using UnityEngine;
using System.Collections;

public class Spike : Segment {
    public float retractDist = -0.5f;
    public float retractSpeed = 0.05f;
    public GameObject spikeBit;

    new void FixedUpdate()
    {
        base.FixedUpdate();
        if (creature)
        {
            if (creature.action)
            {
                spikeBit.transform.localPosition = Vector3.MoveTowards(
                    spikeBit.transform.localPosition,
                    new Vector3(0, 0, 0),
                    retractSpeed);
            }
            else
            {
                spikeBit.transform.localPosition = Vector3.MoveTowards(
                    spikeBit.transform.localPosition,
                    new Vector3(0, retractDist, 0),
                    retractSpeed);
            }
        }
    }

	
}
