using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	// Key objects
	public Player player;
	public MousePlacer mousePlacer;
	// canvas objects
	public Canvas buildCanvas;
	public Canvas playCanvas;

	public GameObject gridCell;
	public GameObject placeable;

	// rotation device components
	public Image previewImage;
	public Button upButton;
	public Button downButton;
	public Button leftButton;
	public Button rightButton;

	// build buttons
	public Button bodyButton;
	public Button mouthButton;
	public Button legButton;
	public Button spikeButton;

	// evo point display
	public Text evoPoints;

	// build warning
	public Text buildWarning;

	// build cost
	public Text bodyPrice;
	public Text mouthPrice;
	public Text legPrice;
	public Text spikePrice;

	// segment variables
	public enum segmentTypes{BODY, MOUTH, LEG, SPIKE};
	public segmentTypes selectedType = segmentTypes.BODY;
	public List<Sprite> segmentSprites;
	public enum directionTypes{UP, DOWN, LEFT, RIGHT};
	public int[] directionValues = new int[4]{0,180,90,270};
	public directionTypes selectedDir = directionTypes.UP;

	// game stats
	public bool build = false;
	int warningTimer = 0;

	// Use this for initialization
	void Start () {
		mousePlacer.sprite = segmentSprites[0];
		previewImage.color = player.playerColor;
		bodyButton.targetGraphic.color = player.playerColor;
		mouthButton.targetGraphic.color = player.playerColor;
		legButton.targetGraphic.color = player.playerColor;

		// add segment button listeners
		bodyButton.onClick.AddListener(() => setSegment(segmentTypes.BODY));
		mouthButton.onClick.AddListener(() => setSegment(segmentTypes.MOUTH));
		legButton.onClick.AddListener(() => setSegment(segmentTypes.LEG));
		spikeButton.onClick.AddListener(() => setSegment(segmentTypes.SPIKE));
		// add direction button listeners
		upButton.onClick.AddListener(() => setDirection(directionTypes.UP));
		downButton.onClick.AddListener(() => setDirection(directionTypes.DOWN));
		leftButton.onClick.AddListener(() => setDirection(directionTypes.LEFT));
		rightButton.onClick.AddListener(() => setDirection(directionTypes.RIGHT));
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("ToggleBuild")) {
			build=!build;
		}

		evoPoints.GetComponent<Text> ().text = player.evo_points.ToString();
		mouthPrice.GetComponent<Text>().text = "20";
		bodyPrice.GetComponent<Text>().text = "10";
		legPrice.GetComponent<Text>().text = "12";
		spikePrice.GetComponent<Text>().text = "15";
	}

	////////////////////
	// BUTTON METHODS //
	////////////////////

	// select a segment, set the sprite for mouse placer and preview image
	public void setSegment(segmentTypes s){
		selectedType = s;
		player.type = (int)s;
		mousePlacer.sprite = segmentSprites [(int)s];
		previewImage.sprite = segmentSprites [(int)s];
		if (s == segmentTypes.SPIKE) {
			previewImage.color = new Color(255,255,255);
			mousePlacer.color = new Color(255,255,255);
		} else {
			previewImage.color = player.playerColor;
			mousePlacer.color = player.playerColor;
		}
	}
	public void setDirection(directionTypes d){
		selectedDir = d;
		player.rot = directionValues [(int)d];
		previewImage.gameObject.transform.rotation = Quaternion.Euler (0, 0, directionValues [(int)d]);
	}
}
