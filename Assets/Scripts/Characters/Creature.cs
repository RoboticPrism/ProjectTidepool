using UnityEngine;
using System.Collections;

public class Creature : MonoBehaviour {

	public Color playerColor = new Color(255,255,255,255);

	//Segment Prefabs
	public GameObject core; //type 0
	public GameObject mouth; //type 1
	public GameObject body; //type 2
	public GameObject spike; //type 3
	public GameObject leg; //type 4
	public int level;

	public Sprite CoreDead;

	//Scalable Size
	public int height = 10;
	public int width = 10;

	//Segment Storage
	protected GameObject[,] segments;
	protected bool[,] placeable;


	//Player stats
	public int health = 10;
	public int tot_health = 10;
	protected float speed = 0;
	protected float rot_speed = 0;
	protected float energy = 0;
	protected float tot_speed = 0;
	protected float tot_rot_speed = 0;
	protected float tot_energy = 0;
	public int exp = 0;
	protected bool dead = false;


	//Creature parts
	public int number_mouth;
	public int number_body;
	public int number_spike;
	public int number_leg;

	public int number_eaten_mouth = 0;
	public int number_eaten_body = 0;
	public int number_eaten_spike = 0;
	public int number_eaten_leg = 0;

	// Use this for initialization
	protected void Start () {
		segments = new GameObject[height*2+1,width*2+1];
		placeable = new bool[height*2+1,width*2+1];
		//Create Core
		segments[height,width] = (GameObject)Instantiate(core, 
		                                                     this.transform.position+new Vector3(0,0,0), 
		                                                     Quaternion.Euler(new Vector3(0,0,0)));
		segments [height, width].transform.parent = transform;
		segments [height, width].GetComponent<Segment> ().creature = this;
		segments [height, width].GetComponent<SpriteRenderer> ().color = playerColor;
		areaSetPlace (height, width, true);
	}
	
	// Update is called once per frame
	protected void Update () {
		if (number_eaten_mouth >= 5) {
			number_mouth++;
			number_eaten_mouth=0;
		}
		if (number_eaten_body >= 5) {
			number_body++;
			number_eaten_body=0;
		}
		if (number_eaten_spike >= 5) {
			number_spike++;
			number_eaten_spike=0;
		}
		if (number_eaten_leg >= 5) {
			number_leg++;
			number_eaten_leg=0;
		}
	}


	//checks if a location is within bounds
	bool boundsCheck(int x, int y){
		if(x<0 || x>=height*2+1 || y<0 || y>=height*2+1){
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
	public void AddSegment(int x, int y, int rot, int type){
		if(checkPlace(x+width,y+height)){
			if(type==1 && number_mouth>0 && checkRot (x+width, y+height,rot)){
				segments[x+width,y+height] = (GameObject)Instantiate(mouth,
				                                                     new Vector2(x+this.transform.position.x,
				            y+this.transform.position.y), 
				                                                     Quaternion.Euler(new Vector3(0,0,rot+transform.rotation.eulerAngles.z)));
				segments[x+width, y+height].transform.parent=this.transform;
				segments[x+width, y+height].GetComponent<Segment>().creature = this;
				segments[x+width, y+height].GetComponent<SpriteRenderer> ().color = playerColor;
				segments[x+width, y+height].transform.localPosition= new Vector2(x,y);
				setPlace (x+width,y+height,false);
				number_mouth--;
			}
			else if(type==2 && number_body>0){
				segments[x+width,y+height] = (GameObject)Instantiate(body,
				                                        new Vector2(x+this.transform.position.x,
				            										y+this.transform.position.y), 
				                                                     Quaternion.Euler(new Vector3(0,0,transform.rotation.eulerAngles.z)));
				segments[x+width, y+height].transform.parent=this.transform;
				segments[x+width, y+height].GetComponent<Segment>().creature = this;
				segments[x+width, y+height].GetComponent<SpriteRenderer> ().color = playerColor;
				segments[x+width, y+height].transform.localPosition= new Vector2(x,y);
				areaSetPlace(x+width,y+height,true);
				setPlace (x+width,y+height,false);
				tot_health+=2;
				number_body--;
			}
			else if(type==3 && number_spike>0 && checkRot (x+width, y+height,rot)){
				segments[x+width,y+height] = (GameObject)Instantiate(spike,
				                                                     new Vector2(x+this.transform.position.x,
				            													 y+this.transform.position.y), 
				                                                     Quaternion.Euler(new Vector3(0,0,rot+transform.rotation.eulerAngles.z)));
				segments[x+width, y+height].transform.parent=this.transform;
				segments[x+width, y+height].GetComponent<Segment>().creature = this;
				segments[x+width, y+height].transform.localPosition= new Vector2(x,y);
				setPlace (x+width,y+height,false);
				number_spike--;
			}
			else if(type==4 && number_leg>0 && checkRot (x+width, y+height,rot)){
				segments[x+width,y+height] = (GameObject)Instantiate(leg,
				                                                     new Vector2(x+this.transform.position.x,
				            													 y+this.transform.position.y), 
				                                                     Quaternion.Euler(new Vector3(0,0,rot+transform.rotation.eulerAngles.z)));
				segments[x+width, y+height].transform.parent=this.transform;
				segments[x+width, y+height].GetComponent<Segment>().creature = this;
				segments[x+width, y+height].GetComponent<SpriteRenderer> ().color = playerColor;
				segments[x+width, y+height].transform.localPosition= new Vector2(x,y);
				setPlace (x+width,y+height,false);
				number_leg--;
				if(rot==0 || rot  == 180){
					tot_rot_speed+=1;
				}
				else{
					tot_speed+=1;
				}
			}
		}
	}

	public void RemoveSegment(int x, int y){
		if (segments [x + width, y + height] != null) {
			if (segments [x + width, y + height].transform.tag == "Mouth") {
				number_mouth++;
				Destroy (segments [x + width, y + height]);
				segments[x+width,y+height]=null;
				setPlace (x + width, y + height, checkNeighbors (x + width, y + height));
				if(boundsCheck(x+width+1,y+height)){
					setPlace (x + width + 1, y + height, checkNeighbors (x + width + 1, y + height));
				}
				if(boundsCheck(x+width-1,y+height)){
					setPlace (x + width - 1, y + height, checkNeighbors (x + width - 1, y + height));
				}
				if(boundsCheck(x+width,y+height+1)){
					setPlace (x + width, y + height + 1, checkNeighbors (x + width, y + height + 1));
				}
				if(boundsCheck(x+width,y+height-1)){
					setPlace (x + width, y + height - 1, checkNeighbors (x + width, y + height - 1));
				}
			}
			else if (segments [x + width, y + height].transform.tag == "Body") {
				tot_health-=2;
				number_body++;
				Destroy (segments [x + width, y + height]);
				segments[x+width,y+height]=null;
				setPlace (x + width, y + height, checkNeighbors (x + width, y + height));
				if(boundsCheck(x+width+1,y+height)){
					setPlace (x + width + 1, y + height, checkNeighbors (x + width + 1, y + height));
				}
				if(boundsCheck(x+width-1,y+height)){
					setPlace (x + width - 1, y + height, checkNeighbors (x + width - 1, y + height));
				}
				if(boundsCheck(x+width,y+height+1)){
					setPlace (x + width, y + height + 1, checkNeighbors (x + width, y + height + 1));
				}
				if(boundsCheck(x+width,y+height-1)){
					setPlace (x + width, y + height - 1, checkNeighbors (x + width, y + height - 1));
				}
			}
			else if (segments [x + width, y + height].transform.tag == "Spike") {
				number_spike++;
				Destroy (segments [x + width, y + height]);
				segments[x+width,y+height]=null;
				setPlace (x + width, y + height, checkNeighbors (x + width, y + height));
				if(boundsCheck(x+width+1,y+height)){
					setPlace (x + width + 1, y + height, checkNeighbors (x + width + 1, y + height));
				}
				if(boundsCheck(x+width-1,y+height)){
					setPlace (x + width - 1, y + height, checkNeighbors (x + width - 1, y + height));
				}
				if(boundsCheck(x+width,y+height+1)){
					setPlace (x + width, y + height + 1, checkNeighbors (x + width, y + height + 1));
				}
				if(boundsCheck(x+width,y+height-1)){
					setPlace (x + width, y + height - 1, checkNeighbors (x + width, y + height - 1));
				}
			}
			else if (segments [x + width, y + height].transform.tag == "Leg") {
				number_leg++;
				Destroy (segments [x + width, y + height]);
				segments[x+width,y+height]=null;
				setPlace (x + width, y + height, checkNeighbors (x + width, y + height));
				if(boundsCheck(x+width+1,y+height)){
					setPlace (x + width + 1, y + height, checkNeighbors (x + width + 1, y + height));
				}
				if(boundsCheck(x+width-1,y+height)){
					setPlace (x + width - 1, y + height, checkNeighbors (x + width - 1, y + height));
				}
				if(boundsCheck(x+width,y+height+1)){
					setPlace (x + width, y + height + 1, checkNeighbors (x + width, y + height + 1));
				}
				if(boundsCheck(x+width,y+height-1)){
					setPlace (x + width, y + height - 1, checkNeighbors (x + width, y + height - 1));
				}
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
		if(health<=0){
			dead = true;
			Die ();
		}
	}

	void Die(){
		segments [width, height].GetComponent<SpriteRenderer> ().sprite = CoreDead;
		segments [width, height].GetComponent<SpriteRenderer> ().sortingOrder = 2;
		for (int x =0; x<height*2+1; x++) {
			for (int y =0; y<width*2+1; y++){
				

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
