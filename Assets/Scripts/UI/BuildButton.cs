using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour {

    private GameController gameController;
    private Player player;
    public bool useColor = true;
    public Sprite image;
    public Button buttonObject;
    public int buildCost = 10;
    public Text buildCostObject;
    public GameController.segmentTypes segmentType;

	// Use this for initialization
	void Start () {
        gameController = FindObjectOfType<GameController>();
        player = FindObjectOfType<Player>();
        buttonObject.onClick.AddListener(() => gameController.setSegment(segmentType));
        setBuildCost(buildCost);
        setImage(image);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setBuildCost(int newBuildCost)
    {
        buildCost = newBuildCost;
        buildCostObject.text = newBuildCost.ToString();
    }

    public void setImage(Sprite newImage)
    {
        image = newImage;
        buttonObject.image.sprite = newImage;
        if (useColor)
        {
            buttonObject.image.color = player.playerColor;
        }
    }
}
