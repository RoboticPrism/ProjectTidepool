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


	//Player stats
	public int health = 10;
	public int tot_health = 10;
	protected float speed = 0;
	protected float rot_speed = 0;
	protected float energy = 0;
	public float tot_speed = 0;
	public float tot_rot_speed = 0;
	protected float tot_energy = 0;
	public int weight = 1;
	public int exp = 0;
	protected bool dead = false;
	protected int damageTimer = 0;
	protected int healTimer = 0;

	public int evoLevel;
    public int evoPoints;

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
		if (damageTimer > 0) {
			damageTimer -= 1;
		} else if (health < tot_health) {
			damageTimer = 0;
			if (healTimer > 0) {
				healTimer -= 1;
			} else {
				health += 1;
				healTimer = 30;
			}
		}
	}

    public virtual void UpdateEvoPoints(int newEvoPoints)
    {
        evoPoints = newEvoPoints;
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
            Debug.Log(buildUnits);
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
        } else
        {
            Debug.Log(buildUnits);
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
            Debug.Log(buildUnits);
            return null;
        }
    }

    // sets the segment at the given build unit location (DOES NOT INSTANTIATE)
    protected void SetSegmentAt(Vector2 buildUnits, Segment newSegment)
    {
        if (BoundsCheck(buildUnits))
        {
            Vector2 arrayUnits = BuildToArrayUnits(buildUnits);
            segments[(int)arrayUnits.x, (int)arrayUnits.y] = newSegment;
        } else
        {
            Debug.Log(buildUnits);
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
    protected bool CheckRot(Vector2 buildUnits, int rot)
    {
        if (rot == 0)
        {
			if(BoundsCheck(buildUnits + Vector2.down)){
				return GetSegmentAt(buildUnits + Vector2.down) != null;
			}
		} 
		if (rot == 90)
        {
            if (BoundsCheck(buildUnits + Vector2.right))
            {
                return GetSegmentAt(buildUnits + Vector2.right) != null;
            }
		} 
		if (rot == 180)
        {
            if (BoundsCheck(buildUnits + Vector2.up))
            {
                return GetSegmentAt(buildUnits + Vector2.up) != null;
            }
        } 
		if (rot == 270)
        {
            if (BoundsCheck(buildUnits + Vector2.left))
            {
                return GetSegmentAt(buildUnits + Vector2.left) != null;
            }
        } 
		return false;
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
	public void AddSegment(Vector2 buildUnits, int rot, Segment segment){
		if(CheckPlace(buildUnits)){
			if(segment && 
                evoPoints > segment.pointCost &&
                (segment.multidirectional || CheckRot(buildUnits, rot)))
            {
				Segment newSegment = ((GameObject)Instantiate(segment.gameObject,
				                                                     (buildUnits + new Vector2(this.transform.position.x, this.transform.position.y)), 
				                                                     Quaternion.Euler(new Vector3(0,0,rot+transform.rotation.eulerAngles.z)))).GetComponent<Segment>();

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
                tot_health += segment.healthBonus;
                tot_speed += segment.speedBonus;
                weight += segment.weightBonus;
			}
		}
	}

    // removes a segment at a location (if possible), updates placeable chart, and updates stats
	public void RemoveSegment(Vector2 buildUnits){
		if (GetSegmentAt(buildUnits) != null) {
            Segment segment = GetSegmentAt(buildUnits);

            // set stats
            UpdateEvoPoints(evoPoints + segment.pointCost);
            tot_health -= segment.healthBonus;
            tot_speed -= segment.speedBonus;
            weight -= segment.weightBonus;

            // destroy gameobject
            Destroy (GetSegmentAt(buildUnits).gameObject);
			SetSegmentAt(buildUnits, null);
            RecalculatePlace(buildUnits);
            RecalculateNeighborPlaces(buildUnits);
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
