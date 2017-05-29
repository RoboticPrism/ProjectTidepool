using UnityEngine;
using System.Collections;

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
	public GameObject[,] segments;
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

	public int evo_points;
	public int evo_level;

	// Use this for initialization
	protected void Start () {
		segments = new GameObject[max_height*2+1,max_width*2+1];
		placeable = new bool[max_height*2+1,max_width*2+1];
		//Create Core
		segments[max_height,max_width] = (GameObject)Instantiate(core, 
		                                                     this.transform.position+new Vector3(0,0,0), 
		                                                     Quaternion.Euler(new Vector3(0,0,0)));
		segments [max_height, max_width].transform.parent = transform;
		segments [max_height, max_width].GetComponent<Segment> ().creature = this;
		segments [max_height, max_width].GetComponent<SpriteRenderer> ().color = playerColor;
		areaSetPlace (max_height, max_width, true);
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


	//checks if a location is within bounds
	bool boundsCheck(int x, int y){
		if(x<0 || x>=max_width*2+1 || y<0 || y>=max_height*2+1){
			return false;
		}
		return true;
	}

	//failsafe check for placeable
	bool checkPlace(int x, int y){
		if(boundsCheck(x,y)){
			return placeable[x,y];
		}
		else{
			return false;
		}
	}

	//checks if the given piece is rotated correctly
	bool checkRot(int x, int y, int rot){
		if (rot == 0) {
			if(boundsCheck(x,y - 1)){
				return segments [x, y - 1] != null;
			}
		} 
		if (rot == 90) {
			if(boundsCheck(x + 1,y)){
				return segments [x + 1, y] != null;
			}
		} 
		if (rot == 180) {
			if(boundsCheck(x,y + 1)){
				return segments [x, y + 1] != null;
			}
		} 
		if (rot == 270) {
			if(boundsCheck(x - 1,y)){
				return segments [x - 1, y] != null;
			}
		} 
		return false;
	}

	//checks a space to see if it has any supporting neighbors, if not, destroy it
    // TODO: Make this actually work
	bool checkNeighbors(int x, int y){
		if (segments [x, y] == null) {
			if (boundsCheck (x, y - 1)) {
				if (segments [x, y - 1] != null) {
					if (segments [x, y - 1].transform.tag == "Body" || segments [x, y - 1].transform.tag == "Core") {
						return true;
					}
				}
			}
			if (boundsCheck (x, y + 1)) {
				if (segments [x, y + 1] != null) {
					if (segments [x, y + 1].transform.tag == "Body" || segments [x, y + 1].transform.tag == "Core") {
						return true;
					}
				}
			}
			if (boundsCheck (x + 1, y)) {
				if (segments [x + 1, y] != null) {
					if (segments [x + 1, y].transform.tag == "Body" || segments [x + 1, y].transform.tag == "Core") {
						return true;
					}
				}
			}
			if (boundsCheck (x - 1, y)) {
				if (segments [x - 1, y] != null) {
					if (segments [x - 1, y].transform.tag == "Body" || segments [x - 1, y].transform.tag == "Core") {
						return true;
					}
				}
			}
		}
		return false;
	}

	//failsafe set for placeable
	void setPlace(int x, int y, bool b){
		if(boundsCheck(x,y)){
			placeable[x,y]=b;
		}
	}

	//make all surrounding unoccupied areas placeable
	void areaSetPlace(int x, int y, bool b){
		if (!checkPlace (x-1,y)) {
			if(boundsCheck(x-1,y)){
				if(segments[x-1,y]==null){
					setPlace(x-1,y,b);
				}
			}
		}
		if (!checkPlace (x+1,y)) {
			if(boundsCheck(x+1,y)){
				if(segments[x+1,y]==null){
					setPlace(x+1,y,b);
				}
			}
		}
		if (!checkPlace (x,y-1)) {
			if(boundsCheck(x,y-1)){
				if(segments[x,y-1]==null){
					setPlace(x,y-1,b);
				}
			}
		}
		if (!checkPlace (x,y+1)) {
			if(boundsCheck(x,y+1)){
				if(segments[x,y+1]==null){
					setPlace(x,y+1,b);
				}
			}
		}
	}


	//Adds a new segment to a location (if possible) and adjusts the placeable chart, returns true if a new item was made
	public void AddSegment(int x, int y, int rot, Segment segment){
		if(checkPlace(x+max_width,y+max_height)){
			if(segment && evo_points > segment.pointCost && (segment.multidirectional || checkRot(x + max_width, y + max_height, rot)))
            {
				segments[x + max_width, y + max_height] = (GameObject)Instantiate(segment.gameObject,
				                                                     new Vector2(x+this.transform.position.x,
				            													 y+this.transform.position.y), 
				                                                     Quaternion.Euler(new Vector3(0,0,rot+transform.rotation.eulerAngles.z)));
				segments[x + max_width, y + max_height].transform.parent=this.transform;
				segments[x + max_width, y + max_height].GetComponent<Segment>().creature = this;
				segments[x + max_width, y + max_height].GetComponent<SpriteRenderer> ().color = playerColor;
				segments[x + max_width, y + max_height].transform.localPosition= new Vector2(x,y);
                if (segment.multidirectional)
                {
                    areaSetPlace(x + max_width, y + max_height, true);
                }
				setPlace (x + max_width, y + max_height,false);
                evo_points -= segment.pointCost;
                tot_health += segment.healthBonus;
                tot_speed += segment.speedBonus;
                weight += segment.weightBonus;
			}
		}
	}

	public void RemoveSegment(int x, int y){
		if (segments [x + max_width, y + max_height] != null) {
            Segment segment = segments[x + max_width, y + max_height].GetComponent<Segment>();
            evo_points += segment.pointCost;
            tot_health -= segment.healthBonus;
            tot_speed -= segment.speedBonus;
            weight -= segment.weightBonus;
            Destroy (segments [x + max_width, y + max_height]);
			segments[x + max_width, y + max_height] =null;
			setPlace (x + max_width, y + max_height, checkNeighbors (x + max_width, y + max_height));
			if(boundsCheck(x + max_width, y + max_height)){
				setPlace (x + max_width + 1, y + max_height, checkNeighbors (x + max_width + 1, y + max_height));
			}
			if(boundsCheck(x + max_width - 1, y + max_height)){
				setPlace (x + max_width - 1, y + max_height, checkNeighbors (x + max_width - 1, y + max_height));
			}
			if(boundsCheck(x + max_width, y + max_height + 1)){
				setPlace (x + max_width, y + max_height + 1, checkNeighbors (x + max_width, y + max_height + 1));
			}
			if(boundsCheck(x + max_width, y + max_height - 1)){
				setPlace (x + max_width, y + max_height - 1, checkNeighbors (x + max_width, y + max_height - 1));
			}
		}
	}

	public bool isOrphaned(int x, int y){
		bool ret = true;
		if (boundsCheck (x+1, y)) {
			if(segments[x+1,y]!=null){
				if(segments[x+1,y].transform.tag=="Core"){
					return false;
				}
				else{
					ret = ret && isOrphaned(x+1,y);
				}
			}
		}
		if (boundsCheck (x-1, y)) {
			if(segments[x-1,y]!=null){
				if(segments[x-1,y].transform.tag=="Core"){
					return false;
				}
				else{
					ret = ret && isOrphaned(x-1,y);
				}
			}
		}
		if (boundsCheck (x, y+1)) {
			if(segments[x,y+1]!=null){
				if(segments[x,y+1].transform.tag=="Core"){
					return false;
				}
				else{
					ret = ret && isOrphaned(x,y+1);
				}
			}
		}
		if (boundsCheck (x, y-1)) {
			if(segments[x,y-1]!=null){
				if(segments[x,y-1].transform.tag=="Core"){
					return false;
				}
				else{
					ret = ret && isOrphaned(x,y-1);
				}
			}
		}
		return ret;
	}

	public void TakeDamage(int damage){
		health -= damage;
		damageTimer = 720;
		if(health<=0){
			dead = true;
			Die ();
		}
	}

	void Die(){
		segments [max_width, max_height].GetComponent<SpriteRenderer> ().sprite = CoreDead;
		segments [max_width, max_height].GetComponent<SpriteRenderer> ().sortingOrder = 2;
		for (int x =0; x<max_height*2+1; x++) {
			for (int y =0; y<max_width*2+1; y++){
				

				if(segments[x,y]!=null){
					GameObject g = (GameObject)Instantiate(segments[x,y], 
					                                       segments[x,y].transform.position, 
					                                       segments[x,y].transform.rotation);
					g.AddComponent<Rigidbody2D>().drag=3;
					g.GetComponent<Segment>().creature=null;
					Destroy (segments[x,y]);
					segments[x,y]=null;
				}
			}
		}
		Destroy (this.gameObject);
	}
}
