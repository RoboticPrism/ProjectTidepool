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
    public RotationPreview rotationPreview;

	// stat displays
	public Text hpPoints;
    public Text energyPoints;
    public Text speedPoints;
	public Text weightPoints;

	// info bars
	public RectTransform healthBar;
    public RectTransform energyBar;
    public GameObject buildReminder;

	// injured image
	public Image injured;
	public int injuredCount = 120;

	// segment variables
	public Creature.rotations selectedRotation = Creature.rotations.UP;

	// game stats
	public bool build = false;

	// Use this for initialization
	void Start () {
		mousePlacer.coloredSprite = player.selectedSegment.coloredSprite;
        mousePlacer.uncoloredSprite = player.selectedSegment.uncoloredSprite;
        mousePlacer.color = player.playerColor;

        rotationPreview.SetSegment(player.selectedSegment, player.playerColor);


        rotationPreview.upButton.onClick.AddListener(() => SetDirection(Creature.rotations.UP));
        rotationPreview.downButton.onClick.AddListener(() => SetDirection(Creature.rotations.DOWN));
        rotationPreview.leftButton.onClick.AddListener(() => SetDirection(Creature.rotations.LEFT));
        rotationPreview.rightButton.onClick.AddListener(() => SetDirection(Creature.rotations.RIGHT));
    }

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("ToggleBuild")) {
			build=!build;
		}
		hpPoints.text = player.totalHealth.ToString();
        energyPoints.text = player.totalEnergy.ToString();
		speedPoints.text = player.totalSpeed.ToString();
		weightPoints.text = player.weight.ToString();
		healthBar.anchorMax = new Vector2 ((float)player.health / (float)player.totalHealth, 1);
        energyBar.anchorMax = new Vector2((float)player.energy / (float)player.totalEnergy, 1);
    }

	void FixedUpdate (){
		if (((float)player.health / (float)player.totalHealth) < .2) {
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
        if (player.evoPoints >= 200)
        {
            buildReminder.SetActive(true);
        } else
        {
            buildReminder.SetActive(false);
        }
	}

	////////////////////
	// BUTTON METHODS //
	////////////////////

	// select a segment, set the sprite for mouse placer and preview image
	public void setSegment(Segment segment){
        player.selectedSegment = segment;

        mousePlacer.SetSegment(segment, player.playerColor);
        rotationPreview.SetSegment(segment, player.playerColor);
	}

    public void SetDirection(Creature.rotations currentRotation)
    {
        player.buildRotation = currentRotation;

        mousePlacer.SetDirection(currentRotation);
        rotationPreview.SetDirection(currentRotation);
    }
}
