using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour {

    private GameController gameController;
    private Player player;
    public Button buttonObject;
    public Text buildCostObject;
    public Segment segmentType;

	// Use this for initialization
	void Start () {
        gameController = FindObjectOfType<GameController>();
        player = FindObjectOfType<Player>();
        buttonObject.onClick.AddListener(() => gameController.setSegment(segmentType));
        buildCostObject.text = segmentType.pointCost.ToString();
        buttonObject.image.sprite = segmentType.sprite;
        if (segmentType.useColor)
        {
            buttonObject.image.color = player.playerColor;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
