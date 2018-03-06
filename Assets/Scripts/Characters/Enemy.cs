using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Creature {
	public GameObject target;
	public bool active = false;
    public bool build = false;

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
        FindTarget();
        if (active  && !build)
        {

            Steering();
            DoAction();
            
            if (evoPoints >= 100)
            {
                BuildEgg();
            }
        } else if (active && build)
        {
            Upgrade();
        }
	}

    void FindTarget()
    {
        GameObject.FindGameObjectWithTag("Player");
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

    void Steering()
    {
        if (target)
        {
            SteerTowards(target);
        }
        else
        {
            speed = 0;
            rotationSpeed = 0;
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
    }

    // run towards a given target
    void SteerTowards(GameObject obj)
    {
        speed = totalSpeed/weight;
        float totalRotationSpeed = totalSpeed * rotationRatio;
        if (obj != null)
        {
            // rotation
            Vector3 targetDir = obj.transform.position - transform.position;
            float angleToTarget = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            float myAngle = transform.rotation.eulerAngles.z;
            // lock to 180, -180
            while (myAngle > 180)
            {
                myAngle -= 360;
            }
            while (myAngle < -180)
            {
                myAngle += 360;
            }
            float angularDiff = angleToTarget - myAngle - 90;
            // lock to 180, -180
            while (angularDiff > 180)
            {
                angularDiff -= 360;
            }
            while (angularDiff < -180)
            {
                angularDiff += 360;
            }
            float angularSpeed;
            if (Mathf.Abs(angularDiff) > 90)
            {
                angularSpeed = totalRotationSpeed * angularDiff/Mathf.Abs(angularDiff);
            } else if (Mathf.Abs(angularDiff) > 15)
            {
                angularSpeed = totalRotationSpeed / 2 * angularDiff / Mathf.Abs(angularDiff);
            } else
            {
                angularSpeed = totalRotationSpeed * angularDiff / 180;
            }
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + angularSpeed));

            // speed
            float dist = Vector3.Distance(obj.transform.position, transform.position);
            if (Mathf.Abs(angularDiff) > 90)
            {
                speed = 0;
            }
            else if (dist > 3)
            {
                speed = totalSpeed;
            } else
            {
                speed = totalSpeed/2;
            }
            transform.Translate(Vector3.up * speed / 50);
        }
    }

    // run away from a given target
    void SteerAway(GameObject obj)
    {

    }

    // Activates spikes and other options while near another creature
    void DoAction()
    {
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

    // randomly builds a new enemy layout
    void Generate()
    {
        int totalEvoPoints = evoPoints;

        // Ensure we don't use pieces above our level
        List<Segment> validBodyPrefabs = new List<Segment>();
        foreach(Segment segment in bodyPrefabs)
        {
            if(segment.level <= level)
            {
                validBodyPrefabs.Add(segment);
            }
        }
        List<Segment> validMainPrefabs = new List<Segment>();
        foreach (Segment segment in mainPrefabs)
        {
            if (segment.level <= level)
            {
                validMainPrefabs.Add(segment);
            }
        }
        List<Segment> validAttackPrefabs = new List<Segment>();
        foreach (Segment segment in attackPrefabs)
        {
            if (segment.level <= level)
            {
                validAttackPrefabs.Add(segment);
            }
        }
        List<Segment> validMovementPrefabs = new List<Segment>();
        foreach (Segment segment in movementPrefabs)
        {
            if (segment.level <= level)
            {
                validMovementPrefabs.Add(segment);
            }
        }
        
        // Generate in a smart enough order
        GenerateParts((int)(totalEvoPoints * 0.3f), validBodyPrefabs);
        
        // Manually generate a mouth on the front, otherwise stuff gets... weird...
        GenerateMouth();
        GenerateParts((int)(totalEvoPoints * 0.1f), validMainPrefabs);
        GenerateParts((int)(totalEvoPoints * 0.3f), validAttackPrefabs);
        GenerateParts((int)(totalEvoPoints * 0.3f), validMovementPrefabs);
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
            Segment randomSegment = segments[Random.Range(0, segments.Count)];
            Vector2 randomBuildSpace = validBuildSpaces[Random.Range(0, validBuildSpaces.Count)];
            rotations randomRotation = 0;
            if (!randomSegment.multidirectional)
            {
                List<rotations> validRotations = ValidRotations(randomBuildSpace, randomSegment);
                if (validRotations.Count > 0)
                {
                    // hard code some priority here to improve symmetry, may revisit this later since we lose a lot of the random elements with this change
                    if(validRotations.Contains(rotations.UP))
                    {
                        randomRotation = rotations.UP;
                    } else if (validRotations.Contains(rotations.DOWN))
                    {
                        randomRotation = rotations.DOWN;
                    } else
                    {
                        randomRotation = validRotations[0];
                    }
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

    void BuildEgg()
    {
        if (eggTime > 0 && !eggObject)
        {
            eggObject = Instantiate(eggPrefab, this.transform.position, this.transform.rotation, this.transform);
            eggObject.transform.localScale = Vector3.zero;
        }
        eggTime += 1;

        if (eggTime >= eggTimeMax)
        {
            build = true;
        }
    }

	void Upgrade()
    {
        level += 1;
        Generate();
        build = false;
        Destroy(this.eggObject);
        Instantiate(brokenEggPrefab, this.transform.position, this.transform.rotation);
    }
}
