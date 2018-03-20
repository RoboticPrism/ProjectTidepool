﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Creature {
	[Header ("State Variables")]
	public bool active = false;
    public bool build = false;
    public enum state { IDLE, ATTACK, RUN, EVOLVE }
    public state currentState = state.IDLE;

    public GameObject target;
    protected List<Stimulus> nearbyStimuli = new List<Stimulus>();

    [Header("Prefab Connections")]
    public Segment corePrefab;
    public Segment mouthPrefab;

    [Header("Colors")]
    public List<Color> levelColors;

    [Header("Segment List")]
    public List<Segment> bodyPrefabs;
    public List<Segment> mainPrefabs;
    public List<Segment> attackPrefabs;
    public List<Segment> defensePrefabs;
    public List<Segment> movementPrefabs;

    [Header("Generation Variables")]
    [Tooltip("How much of the allotted budget will be spent on body blocks")]
    [Range(0f,1f)]
    public float bodyRatio = 0.3f;
    [Tooltip("How much of the allotted budget will be spent on main segments")]
    [Range(0f, 1f)]
    public float mainRatio = 0.1f;
    [Tooltip("How much of the allotted budget will be spent on attack segments")]
    [Range(0f, 1f)]
    public float attackRatio = 0.3f;
    [Tooltip("How much of the allotted budget will be spent on movement segments")]
    [Range(0f, 1f)]
    public float movementRatio = 0.3f;

    [Header("AI Behavior Variables")]
    [Tooltip("Multiplier for how much AI prioritizes safety to rewards")]
    public float threatToWorthMultiplier = 10f;
    [Tooltip("Multiplier for how much AI prioritizes evolving to being safe")]
    public float threatToEvolveMultiplier = 10f;
    [Tooltip("Multiplier for how much more threatening everything appears when at low health")]
    public float damageThreatMultiplier = 10f;
    [Tooltip("The threshold of evo points before an AI starts looking to evolve")]
    public int evolveThreshold = 100;    

    // Use this for initialization
    new void Start () {
		base.Start ();
        totalEnergy = 1;
        UpdateColor();
        Generate();
	}
	
	// Update is called once per frame
	new void FixedUpdate () {
		base.FixedUpdate ();
        if (active)
        {
            FindTarget();
            switch (currentState)
            {
                case state.IDLE:
                    //do nothing
                    break;
                case state.ATTACK:
                    SteerTowards(target.gameObject);
                    break;
                case state.RUN:
                    SteerAway(target.gameObject);
                    break;
                case state.EVOLVE:
                    if (build)
                    {
                        Upgrade();
                    }
                    else
                    {
                        BuildEgg();
                    }
                    break;
            }
        }
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        Stimulus s = col.GetComponent<Stimulus>();
        if (s)
        {
            nearbyStimuli.Add(s);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Stimulus s = col.GetComponent<Stimulus>();
        if (s)
        {
            nearbyStimuli.Remove(s);
        }
    }

    void FindTarget()
    {
        float currentHunger = 0; // total point worth in the area
        float currentThreat = 0; // total threat value of the area
        Stimulus bestWorth = null;
        Stimulus worstThreat = null;

        // remove any stimuli that may have gotten deleted (i.e. eaten)
        nearbyStimuli.RemoveAll(stim => stim == null);

        // evaluate local area
        foreach (Stimulus stimulus in nearbyStimuli)
        {
            float radius = GetComponent<CircleCollider2D>().radius;
            float sWorth = stimulus.worth * (radius/Vector3.Distance(stimulus.transform.position, transform.position));
            float sThreat = stimulus.threat * (radius/Vector3.Distance(stimulus.transform.position, transform.position));

            if (bestWorth == null || bestWorth.worth < stimulus.worth)
            {
                bestWorth = stimulus;
            }
            if (worstThreat == null || worstThreat.threat < stimulus.threat)
            {
                worstThreat = stimulus;
            }

            currentHunger += sWorth;
            currentThreat += sThreat;
        }

        // if low health, threats appear worse
        float missingHealthPerc = (totalHealth - health) / totalHealth;
        currentThreat = missingHealthPerc * damageThreatMultiplier;

        // make state swap based on evaluation
        if (evoPoints >= evolveThreshold && currentThreat * threatToEvolveMultiplier < evoPoints)
        {
            currentState = state.EVOLVE;
        }
        else if (bestWorth && currentThreat * threatToWorthMultiplier < currentHunger)
        {
            target = bestWorth.gameObject;
            currentState = state.ATTACK;
        }
        else if (worstThreat)
        {
            target = worstThreat.gameObject;
            currentState = state.RUN;
        }
        else
        {
            target = null;
            currentState = state.IDLE;
        }

        // perform actions when close to target
        if (target && Vector3.Distance(target.transform.position, transform.position) < 10f && energy > 0.5f)
        {
            action = true;
        }
        else if (energy <= 0)
        {
            action = false;
        }
    }

    // run towards a given target
    void SteerTowards(GameObject obj)
    {
        speed = Mathf.Max(1f, totalSpeed/weight);
        float totalRotationSpeed = speed * rotationRatio;
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
            float angularDiff = angleToTarget - myAngle - 90; // -90 because unity puts 0 as up
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
        speed = totalSpeed / weight;
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
            float angularDiff = angleToTarget - myAngle + 90; // + 90 because unity puts 0 as up
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
                angularSpeed = totalRotationSpeed * angularDiff / Mathf.Abs(angularDiff);
            }
            else if (Mathf.Abs(angularDiff) > 15)
            {
                angularSpeed = totalRotationSpeed / 2 * angularDiff / Mathf.Abs(angularDiff);
            }
            else
            {
                angularSpeed = totalRotationSpeed * angularDiff / 180;
            }
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + angularSpeed));

            // speed
            float dist = Vector3.Distance(obj.transform.position, transform.position);
            speed = totalSpeed;
            transform.Translate(Vector3.up * speed / 50);
        }
    }

    // Activates spikes and other options while near another creature
    void DoAction()
    {
        if (target && target.GetComponent<Creature>() && Vector3.Distance(this.transform.position, target.transform.position) < 5)
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
        GenerateParts((int)(totalEvoPoints * bodyRatio), validBodyPrefabs);
        
        // Manually generate a mouth on the front, otherwise stuff gets... weird...
        GenerateMouth();
        GenerateParts((int)(totalEvoPoints * mainRatio), validMainPrefabs);
        GenerateParts((int)(totalEvoPoints * attackRatio), validAttackPrefabs);
        GenerateParts((int)(totalEvoPoints * movementRatio), validMovementPrefabs);
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
        UpdateColor();
        Generate();
        build = false;
        Destroy(this.eggObject);
        Instantiate(brokenEggPrefab, this.transform.position, this.transform.rotation);
    }

    protected void UpdateColor()
    {
        if (level >= 0 && level < levelColors.Count)
        {
            UpdateColor(levelColors[level]);
        }
    }
}
