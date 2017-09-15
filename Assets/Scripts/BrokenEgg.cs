using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenEgg : MonoBehaviour {

    public float maxLifeTime = 30;
    private int lifeTime;
    public int bustForce;
    List<SpriteRenderer> pieces = new List<SpriteRenderer>();

	// Use this for initialization
	void Start () {
        lifeTime = (int)maxLifeTime;
		foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(-bustForce, bustForce));
            pieces.Add(child.gameObject.GetComponent<SpriteRenderer>());
        }
	}
	
	void FixedUpdate () {
        lifeTime -= 1;
        foreach (SpriteRenderer piece in pieces)
        {
            piece.color = new Color(1f, 1f, 1f , lifeTime / maxLifeTime);
        }
        if (lifeTime <= 0)
        {
            Destroy(this);
        }
	}
}
