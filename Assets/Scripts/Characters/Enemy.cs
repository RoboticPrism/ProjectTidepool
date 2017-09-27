using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Creature {
	public GameObject target;
	public bool active=false;

    public Segment corePrefab;
    public Segment mouthPrefab;
    public List<Segment> bodyPrefabs;
    public List<Segment> mainPrefabs;
    public List<Segment> attackPrefabs;
    public List<Segment> defensePrefabs;
    public List<Segment> movementPrefabs;

    // Use this for initialization
    new void Start () {
		base.Start ();
        totalEnergy = 1;
        Generate();
	}
	
	// Update is called once per frame
	new void FixedUpdate () {
		base.FixedUpdate ();
        // find a target
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
        if (active)
        {
            speed = totalSpeed;
            float totalRotationSpeed = totalSpeed * rotationRatio;
            if (target != null)
            {
                if (this.level < target.GetComponent<Creature>().level)
                {
                    if (target != null)
                    {
                        Vector3 targetDir = target.transform.position - transform.position;
                        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
                        Quaternion q = Quaternion.AngleAxis(angle + 90, Vector3.forward);
                        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * totalRotationSpeed / 10);
                        float offset = Vector3.Distance(target.transform.position, transform.position);
                        if (offset > 50)
                        {
                            speed = 0;
                        }
                        else
                        {
                            speed = totalSpeed;
                        }
                        transform.Translate(Vector3.up * speed / 100);
                    }
                }
                else
                {
                    if (target != null)
                    {
                        // movement and rotation
                        Vector3 targetDir = target.transform.position - transform.position;
                        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
                        Quaternion q = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * totalRotationSpeed / 10);
                        float offrot = Mathf.Abs(angle - 90 - transform.rotation.eulerAngles.z);
                        if (offrot > 180)
                        {
                            offrot = 360 - offrot;
                        }
                        if (offrot > 90)
                        {
                            speed = 0;
                        }
                        else
                        {
                            speed = totalSpeed * (1 - offrot / 90);
                        }
                        transform.Translate(Vector3.up * speed / 100);
                        // action activation
                        if (target.GetComponent<Creature>() && Vector3.Distance(this.transform.position, target.transform.position) < 5)
                        {
                            if (energy > 0)
                            {
                                action = true;
                            }
                            else
                            {
                                action = false;
                            }
                        }
                        else
                        {
                            action = false;
                        }
                    }

                }
            }
            else
            {
                speed = 0;
                rotationSpeed = 0;
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
            if (evoPoints >= 100)
            {
                Generate();
            }
        }
	}

    // randomly builds a new enemy layout
    void Generate()
    {
        int totalEvoPoints = evoPoints;
        GenerateParts((int)(totalEvoPoints * 0.3f), bodyPrefabs);
        // Manually generate a mouth on the front, otherwise stuff gets... weird...
        GenerateMouth();
        GenerateParts((int)(totalEvoPoints * 0.1f), mainPrefabs);
        GenerateParts((int)(totalEvoPoints * 0.3f), attackPrefabs);
        GenerateParts((int)(totalEvoPoints * 0.3f), movementPrefabs);
    }

    // generates a copy of an existing enemy layout
    void GenerateExisting(Enemy enemy)
    {

    }

    // special generation to make sure a mouth always generates
    void GenerateMouth() {
        foreach (Vector2 validBuildSpace in ValidBuildSpaces())
        {
            if (validBuildSpace.x == 0 && CheckRot(validBuildSpace, rotations.UP)) {
                AddSegment(validBuildSpace, rotations.UP, mouthPrefab);
                break;
            }
        }
    }

    // randomly adds the given parts within the budget
    void GenerateParts(int evoPointAllowance, List<Segment> segments)
    {
        List<Vector2>validBuildSpaces = ValidBuildSpaces();
        int maxTries = 10;
        while(evoPointAllowance > 0 && validBuildSpaces.Count > 0 && segments.Count > 0 && maxTries > 0)
        {
            // Get our random values together
            Segment randomSegment = segments[Random.Range(0, segments.Count - 1)];
            Vector2 randomBuildSpace = validBuildSpaces[Random.Range(0, validBuildSpaces.Count - 1)];
            rotations randomRotation = 0;
            if (!randomSegment.multidirectional)
            {
                List<rotations> validRotations = ValidRotations(randomBuildSpace, randomSegment);
                if (validRotations.Count > 0)
                {
                    randomRotation = validRotations[Random.Range(0, validRotations.Count - 1)];
                }
            }

            // See if we are building two items or one and make sure we can afford it
            if ((randomBuildSpace.x == 0 && evoPointAllowance > randomSegment.pointCost) ||
                (randomBuildSpace.x != 0 && evoPointAllowance > randomSegment.pointCost * 2))
            {
                AddSegmentYSymetrical(randomBuildSpace,
                    randomRotation,
                    randomSegment);
                if (randomBuildSpace.x == 0)
                {
                    evoPointAllowance -= randomSegment.pointCost;
                } else
                {
                    evoPointAllowance -= (randomSegment.pointCost * 2);
                }
                validBuildSpaces = ValidBuildSpaces();
            }
            maxTries--;
        }
    }

    

	void Upgrade(){

	}
}
