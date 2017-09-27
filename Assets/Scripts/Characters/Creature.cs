using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Creature : MonoBehaviour {

	public Color playerColor = new Color(255,255,255,255);

	//Segment Prefabs
	public GameObject core;
	public int level;

	public Sprite CoreDead;

	//Scalable Size
	public int height = 5;
	public int width = 5;
    public int max_height = 5;
    public int max_width = 5;

	//Segment Storage
	public Segment[,] segments;
	public bool[,] placeable;

    // Egg
    public GameObject eggObject;
    public GameObject eggPrefab;
    public GameObject brokenEggPrefab;
    public int eggTime = 0;
    public int eggTimeMax = 60;
    public float eggScale = 2f;

	//Player stats
	public int health = 10;
	public int totalHealth = 10;
    public float energy = 0;
    public float totalEnergy = 0;
    protected float speed = 0;
    protected float rotationSpeed = 0;
    public float totalSpeed = 0;
    public float rotationRatio = 0.5f;
	public int weight = 1;
	public int exp = 0;
	protected bool dead = false;
	protected int damageTimer = 0;
	protected int healTimer = 0;

    public bool action = false;

	public int evoLevel;
    public int evoPoints;

    public enum rotations { UP, DOWN, LEFT, RIGHT}

    // used by orphaned piece checker
    List<Segment> checkedSegments;

    // Use this for initialization
    protected void Start () {
		segments = new Segment[max_height*2+1,max_width*2+1];
		placeable = new bool[max_height*2+1,max_width*2+1];
		//Create Core
		segments[max_height,max_width] = ((GameObject)Instantiate(core, 
		                                                     this.transform.position+new Vector3(0,0,0), 
		                                                     Quaternion.Euler(new Vector3(0,0,0)))).GetComponent<Segment>();
		segments [max_height, max_width].transform.parent = transform;
		segments [max_height, max_width].GetComponent<Segment> ().creature = this;
		segments [max_height, max_width].GetComponent<SpriteRenderer> ().color = playerColor;
        RecalculatePlace(Vector2.zero);
        RecalculateNeighborPlaces(Vector2.zero);
    }

	public void Update(){
        
	}

	// Update is called once per frame
	protected void FixedUpdate () {
        // health
        if (damageTimer > 0) {
			damageTimer -= 1;
		} else if (health < totalHealth) {
			damageTimer = 0;
			if (healTimer > 0) {
				healTimer -= 1;
			} else {
				health += 1;
				healTimer = 30;
			}
		}
        // energy
        if (action)
        {
            energy -= 0.005f;
            if (energy <= 0)
            {
                energy = 0f;
                action = false;
            }
        } else if (energy < totalEnergy) {
            energy += 0.005f;
        }
        if (energy > totalEnergy)
        {
            energy = totalEnergy;
        }

        // egg scaling
        if (eggObject)
        {
            eggObject.transform.localScale = new Vector3(eggTime * eggScale / eggTimeMax,
                                                         eggTime * eggScale / eggTimeMax,
                                                         eggTime * eggScale / eggTimeMax);
        }
    }

    public virtual void UpdateEvoPoints(int newEvoPoints)
    {
        evoPoints = newEvoPoints;
    }

    ///////////////////////
    // ROTATION UTLITIES //
    ///////////////////////

    // converts a rotation to world space degrees
    public static int RotationToInt(rotations currentRotation)
    {
        switch (currentRotation)
        {
            case rotations.UP:
                return 0;
            case rotations.DOWN:
                return 180;
            case rotations.LEFT:
                return 90;
            case rotations.RIGHT:
                return 270;
            default:
                return 0;
        }
    }

    // converts world space degrees to a rotation, defaults to UP if not valid
    public static rotations IntToRotation(int degrees)
    {
        switch (degrees)
        {
            case 0:
                return rotations.UP;
            case 180:
                return rotations.DOWN;
            case 90:
                return rotations.LEFT;
            case 270:
                return rotations.RIGHT;
            default:
                return rotations.UP;
        }
    }

    ////////////////////
    // GRID UTILITIES //
    ////////////////////

    // transforms from array units (0 to 2max) to build units (-max to max) 
    protected Vector2 ArrayToBuildUnits(Vector2 arrayUnits)
    {
        return new Vector2(arrayUnits.x - max_width, arrayUnits.y - max_height);
    }

    // transforms from build units (-max to max) to array units (0 to 2max)
    protected Vector2 BuildToArrayUnits(Vector2 buildUnits)
    {
        return new Vector2(buildUnits.x + max_width, buildUnits.y + max_height);
    }

    // gets the placeable value at a given build location
    protected bool GetPlaceable(Vector2 buildUnits)
    {
        if (BoundsCheck(buildUnits))
        {
            Vector2 arrayUnits = BuildToArrayUnits(buildUnits);
            return placeable[(int)arrayUnits.x, (int)arrayUnits.y];
        } else
        {
            return false;
        }
    }

    // sets the placeable value at a given build location
    protected void SetPlaceable(Vector2 buildUnits, bool newValue)
    {
        if (BoundsCheck(buildUnits))
        {
            Vector2 arrayUnits = BuildToArrayUnits(buildUnits);
            placeable[(int)arrayUnits.x, (int)arrayUnits.y] = newValue;
        }
    }

    // gets the segment at the given build unit location
    protected Segment GetSegmentAt(Vector2 buildUnits)
    {
        if (BoundsCheck(buildUnits))
        {
            Vector2 arrayUnits = BuildToArrayUnits(buildUnits);
            return segments[(int)arrayUnits.x, (int)arrayUnits.y];
        } else
        {
            return null;
        }
    }

    // sets the segment points to an instantiated object at the given build unit location (DOES NOT INSTANTIATE)
    protected void SetSegmentAt(Vector2 buildUnits, Segment newSegment)
    {
        if (BoundsCheck(buildUnits))
        {
            Vector2 arrayUnits = BuildToArrayUnits(buildUnits);
            segments[(int)arrayUnits.x, (int)arrayUnits.y] = newSegment;
        }
    }

    //////////////////////////////
    // GRID PLACEABLE UTILITIES //
    //////////////////////////////

    // checks if a location is within bounds
    protected bool BoundsCheck(Vector2 buildUnits)
    {
		if(buildUnits.x > width ||
            buildUnits.x < -width ||
            buildUnits.y > height ||
            buildUnits.y < -height)
        {
			return false;
		}
		return true;
	}

    // failsafe check for placeable
    protected bool CheckPlace(Vector2 buildUnits)
    {
		if(BoundsCheck(buildUnits))
        {
            return GetPlaceable(buildUnits);
		} else
        {
			return false;
		}
	}

    // checks if the given piece is valid with its given rotation
    protected bool CheckRot(Vector2 buildUnits, rotations currentRotation)
    {
        Segment segment;
        switch (currentRotation)
        {
            case rotations.UP:
                segment = GetSegmentAt(buildUnits + Vector2.down);
                return segment != null && segment.multidirectional;
            case rotations.DOWN:
                segment = GetSegmentAt(buildUnits + Vector2.up);
                return segment != null && segment.multidirectional;
            case rotations.LEFT:
                segment = GetSegmentAt(buildUnits + Vector2.right);
                return segment != null && segment.multidirectional;
            case rotations.RIGHT:
                segment = GetSegmentAt(buildUnits + Vector2.left);
                return segment != null && segment.multidirectional;
        }
		return false;
	}

    // returns valid rotations for this segment in its current location
    public List<rotations> ValidRotations(Vector2 buildUnits, Segment segment)
    {
        List<rotations> allRotations = new List<rotations> { rotations.UP, rotations.DOWN, rotations.LEFT, rotations.RIGHT };
        if (CheckPlace(buildUnits))
        {
            if (segment.multidirectional)
            {
                return allRotations;
            } else
            {
                List<rotations> returnList = new List<rotations>();
                foreach (rotations rot in allRotations)
                {
                    if(CheckRot(buildUnits, rot))
                    {
                        returnList.Add(rot);
                    }
                }
                return returnList;
            }
        } else
        {
            return new List<rotations>();
        }
    }

    // sets the given build location's placeable to true or false
    protected void SetPlace(Vector2 buildUnits, bool b)
    {
		if(BoundsCheck(buildUnits))
        {
            SetPlaceable(buildUnits, b);
		}
	}

    // recalculate if this build location should be placeable or not
    protected void RecalculatePlace(Vector2 buildUnits)
    {
        bool isPlaceable = false;
        // if something is here, its not buildable, otherwise check if multidirectional neighbors make this supportable
        if (GetSegmentAt(buildUnits) == null)
        {
            if (BoundsCheck(buildUnits + Vector2.up) &&
                GetSegmentAt(buildUnits + Vector2.up) != null &&
                GetSegmentAt(buildUnits + Vector2.up).multidirectional)
            {
                isPlaceable = true;
            }
            if (BoundsCheck(buildUnits + Vector2.down) &&
                GetSegmentAt(buildUnits + Vector2.down) != null &&
                GetSegmentAt(buildUnits + Vector2.down).multidirectional)
            {
                isPlaceable = true;
            }
            if (BoundsCheck(buildUnits + Vector2.left) &&
                GetSegmentAt(buildUnits + Vector2.left) != null &&
                GetSegmentAt(buildUnits + Vector2.left).multidirectional)
            {
                isPlaceable = true;
            }
            if (BoundsCheck(buildUnits + Vector2.right) &&
                GetSegmentAt(buildUnits + Vector2.right) != null &&
                GetSegmentAt(buildUnits + Vector2.right).multidirectional)
            {
                isPlaceable = true;
            }
        }
        SetPlace(buildUnits, isPlaceable);
	}

    // recaluclates neighbors build locations
    protected void RecalculateNeighborPlaces(Vector2 buildUnits)
    {
        RecalculatePlace(buildUnits + Vector2.up);
        RecalculatePlace(buildUnits + Vector2.down);
        RecalculatePlace(buildUnits + Vector2.left);
        RecalculatePlace(buildUnits + Vector2.right);
    }

    //////////////////////////////////
    // SEGMENT CREATION AND REMOVAL //
    //////////////////////////////////

	// adds a new segment to a location (if possible), adjusts the placeable chart and adjusts stats
	public void AddSegment(Vector2 buildUnits, rotations currentRotation, Segment segment){
		if(CheckPlace(buildUnits)){
			if(segment && 
                evoPoints > segment.pointCost &&
                (segment.multidirectional || CheckRot(buildUnits, currentRotation)))
            {
				Segment newSegment = ((GameObject)Instantiate(segment.gameObject,
				                                                     (buildUnits + new Vector2(this.transform.position.x, this.transform.position.y)), 
				                                                     Quaternion.Euler(new Vector3(0, 0, RotationToInt(currentRotation)+transform.rotation.eulerAngles.z)))).GetComponent<Segment>();

                // set location
                newSegment.transform.parent = this.transform;
				newSegment.creature = this;
                newSegment.transform.localPosition = new Vector2(buildUnits.x, buildUnits.y);

                // add segment to the array
                SetSegmentAt(buildUnits, newSegment);

                // set placeable locations
                RecalculatePlace(buildUnits);
                RecalculateNeighborPlaces(buildUnits);
                
                // set color
                if (segment.useColor)
                {
                    newSegment.GetComponent<SpriteRenderer>().color = playerColor;
                }

                // set stats
                UpdateEvoPoints(evoPoints - segment.pointCost);
                totalHealth += segment.healthBonus;
                totalSpeed += segment.speedBonus;
                weight += segment.weightBonus;
			}
		}
	}

    // Adds a new segment to the given location and also adds a new segment mirrored along the y axis
    public void AddSegmentYSymetrical(Vector2 buildUnits, rotations currentRotation, Segment segment)
    {
        if (buildUnits.x == 0)
        {
            AddSegment(buildUnits, currentRotation, segment);
        }
        else
        {
            AddSegment(buildUnits, currentRotation, segment);
            rotations _altRot = currentRotation;
            if (currentRotation == rotations.LEFT)
            {
                _altRot = rotations.RIGHT;
            }
            else if (currentRotation == rotations.RIGHT)
            {
                _altRot = rotations.LEFT;
            }
            AddSegment(new Vector2(-buildUnits.x, buildUnits.y), _altRot, segment);
        }
    }

    // removes a segment at a location (if possible), updates placeable chart, and updates stats, runs orphaned piece check
    public void RemoveSegment(Vector2 buildUnits){
        RemoveSegmentWithoutChecks(buildUnits);
        CheckForOrphanedPieces();
	}

    // remove segment but without the orphaned piece check, for use with orphaned piece check cleanup
    void RemoveSegmentWithoutChecks(Vector2 buildUnits)
    {
        if (GetSegmentAt(buildUnits) != null)
        {
            Segment segment = GetSegmentAt(buildUnits);

            // set stats
            UpdateEvoPoints(evoPoints + segment.pointCost);
            totalHealth -= segment.healthBonus;
            totalSpeed -= segment.speedBonus;
            weight -= segment.weightBonus;

            // destroy gameobject
            Destroy(GetSegmentAt(buildUnits).gameObject);
            SetSegmentAt(buildUnits, null);
            RecalculatePlace(buildUnits);
            RecalculateNeighborPlaces(buildUnits);
        }
    }

    //////////////////////////
    // ORPHANED PIECE CHECK //
    //////////////////////////

    // finds and removes pieces that are no longer connected to the core
    void CheckForOrphanedPieces()
    {
        checkedSegments = new List<Segment>();
        
        // set all peices to orphaned
        foreach (Segment segment in segments)
        {
            if (segment != null)
            {
                segment.attachedToCore = false;
            }
        }

        // from the core, expand outwards 
        CheckIfOrphanedPiece(Vector2.zero);

        // delete all pieces that aren't attached to the core
        for (int x = 0; x < segments.GetLength(0); x++)
        {
            for (int y = 0; y < segments.GetLength(1); y++)
            {
                Segment currentSegment = GetSegmentAt(ArrayToBuildUnits(new Vector2(x, y)));
                if(currentSegment != null &&
                    !currentSegment.attachedToCore)
                {
                    RemoveSegmentWithoutChecks(ArrayToBuildUnits(new Vector2(x, y)));
                }
            }
        }
    }

    // checks if an individual piece is attached to the core 
    void CheckIfOrphanedPiece(Vector2 buildUnits)
    {
        // if segment is core, or is adjacent to an item that is attached to the core, its attached to the core
        Segment currentSegment = GetSegmentAt(buildUnits);
        if (currentSegment.GetComponent<Core>() != null)
        {
            currentSegment.attachedToCore = true;
        } else if (
            (GetSegmentAt(buildUnits + Vector2.up) != null && GetSegmentAt(buildUnits + Vector2.up).attachedToCore) ||
            (GetSegmentAt(buildUnits + Vector2.down) != null && GetSegmentAt(buildUnits + Vector2.down).attachedToCore) ||
            (GetSegmentAt(buildUnits + Vector2.left) != null && GetSegmentAt(buildUnits + Vector2.left).attachedToCore) ||
            (GetSegmentAt(buildUnits + Vector2.right) != null && GetSegmentAt(buildUnits + Vector2.right).attachedToCore))
        {
            currentSegment.attachedToCore = true;
        }

        // state that we have checked this segment
        checkedSegments.Add(currentSegment);

        // check all adjacent if pieces exist there and they haven't been checked yet
        if (GetSegmentAt(buildUnits + Vector2.up) != null &&
            !checkedSegments.Contains(GetSegmentAt(buildUnits + Vector2.up)))
        {
            CheckIfOrphanedPiece(buildUnits + Vector2.up);
        }
        if (GetSegmentAt(buildUnits + Vector2.down) != null &&
            !checkedSegments.Contains(GetSegmentAt(buildUnits + Vector2.down)))
        {
            CheckIfOrphanedPiece(buildUnits + Vector2.down);
        }
        if (GetSegmentAt(buildUnits + Vector2.left) != null &&
            !checkedSegments.Contains(GetSegmentAt(buildUnits + Vector2.left)))
        {
            CheckIfOrphanedPiece(buildUnits + Vector2.left);
        }
        if (GetSegmentAt(buildUnits + Vector2.right) != null &&
            !checkedSegments.Contains(GetSegmentAt(buildUnits + Vector2.right)))
        {
            CheckIfOrphanedPiece(buildUnits + Vector2.right);
        }
    }

    // returns a list of valid build spaces in build units
    public List<Vector2> ValidBuildSpaces()
    {
        List<Vector2> returnList = new List<Vector2>();
        for (int i = 0;  i < placeable.GetLength(0); i++)
        {
            for (int j = 0; j < placeable.GetLength(1); j++)
            {
                if (placeable[i, j])
                {
                    returnList.Add(ArrayToBuildUnits(new Vector2(i, j)));
                }
            }
        }
        return returnList;
    }

    // inflict damage on this creature
	public void TakeDamage(int damage){
		health -= damage;
		damageTimer = 720;
        eggTime = 0;
		if(health <= 0){
			dead = true;
			Die ();
		}
	}

    // kill this creature and make its body explode into pieces
	void Die(){
		GetSegmentAt(Vector2.zero).GetComponent<SpriteRenderer> ().sprite = CoreDead;
        GetSegmentAt(Vector2.zero).GetComponent<SpriteRenderer> ().sortingOrder = 2;
		for (int x = 0; x < placeable.GetLength(0); x++) {
			for (int y = 0; y < placeable.GetLength(1); y++){

                Segment _currentSegement = GetSegmentAt(ArrayToBuildUnits(new Vector2(x, y)));
                if (_currentSegement != null){
					GameObject g = (GameObject)Instantiate(_currentSegement.gameObject,
                                                           _currentSegement.transform.position,
                                                           _currentSegement.transform.rotation);
					g.AddComponent<Rigidbody2D>().drag = 3;
					g.GetComponent<Segment>().creature = null;
					Destroy (_currentSegement);
                    SetSegmentAt(ArrayToBuildUnits(new Vector2(x, y)), null);
                }
			}
		}
		Destroy (this.gameObject);
	}
}
