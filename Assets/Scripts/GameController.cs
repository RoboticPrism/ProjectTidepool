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

	// stat displays
	public Text evoPoints;
	public Text hpPoints;
	public Text speedPoints;
	public Text rSpeedPoints;
	public Text weightPoints;

	// build warning
	public Text buildWarning;

	// health bar
	public RectTransform healthBar;

	// injured image
	public Image injured;
	public int injuredCount = 120;

	// segment variables
	public Creature.rotations selectedRotation = Creature.rotations.UP;

	// game stats
	public bool build = false;

	// Use this for initialization
	void Start () {
		mousePlacer.sprite = player.selectedSegment.sprite;
        mousePlacer.color = player.playerColor;
		previewImage.color = player.playerColor;

		// add direction button listeners
		upButton.onClick.AddListener(() => setDirection(Creature.rotations.UP));
		downButton.onClick.AddListener(() => setDirection(Creature.rotations.DOWN));
		leftButton.onClick.AddListener(() => setDirection(Creature.rotations.LEFT));
		rightButton.onClick.AddListener(() => setDirection(Creature.rotations.RIGHT));
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("ToggleBuild")) {
			build=!build;
		}
		hpPoints.text = player.tot_health.ToString();
		speedPoints.text = player.tot_speed.ToString();
		rSpeedPoints.text = player.tot_rot_speed.ToString();
		weightPoints.text = player.weight.ToString();
		healthBar.anchorMax = new Vector2 ((float)player.health / (float)player.tot_health, 1);
	}

	void FixedUpdate (){
		if (((float)player.health / (float)player.tot_health) < .2) {
			injured.gameObject.SetActive (true);
			if (injuredCount <= 0) {
				injuredCount = 120;
			} else {
				injuredCount -= 1;
			}
			injured.color = new Color (1, 1, 1, (float)injuredCount/240f );
		} else {
			injured.gameObject.SetActive (false);
		}
	}

	////////////////////
	// BUTTON METHODS //
	////////////////////

	// select a segment, set the sprite for mouse placer and preview image
	public void setSegment(Segment segment){
		player.selectedSegment = segment;
		mousePlacer.sprite = player.selectedSegment.sprite;
		previewImage.sprite = player.selectedSegment.sprite;
		if (segment.useColor) {
            previewImage.color = player.playerColor;
            mousePlacer.color = player.playerColor;
		} else {
            previewImage.color = new Color(255, 255, 255);
            mousePlacer.color = new Color(255, 255, 255);
        }
	}
	public void setDirection(Creature.rotations currentRotation){
        player.buildRotation = currentRotation;
		previewImage.gameObject.transform.localEulerAngles = new Vector3(0, 0, Creature.RotationToInt(currentRotation));
	}
}
