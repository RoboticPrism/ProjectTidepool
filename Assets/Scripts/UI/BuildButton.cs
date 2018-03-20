using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour {

    private GameController gameController;
    private Player player;
    public Button buttonObject;
    public Image uncoloredObject;
    public Text buildCostObject;
    public Segment segmentType;

	// Use this for initialization
	void Start () {
        gameController = FindObjectOfType<GameController>();
        player = FindObjectOfType<Player>();
        buttonObject.onClick.AddListener(() => gameController.setSegment(segmentType));
        buildCostObject.text = segmentType.pointCost.ToString();

        if (segmentType.coloredSprite)
        {
            buttonObject.image.sprite = segmentType.coloredSprite;
            buttonObject.image.color = player.color;
        } else
        {
            buttonObject.image.sprite = null;
            buttonObject.image.color = new Color(0, 0, 0, 0);
        }
        if (segmentType.uncoloredSprite)
        {
            uncoloredObject.sprite = segmentType.uncoloredSprite;
            uncoloredObject.color = Color.white;
        }
        else
        {
            uncoloredObject.sprite = null;
            uncoloredObject.color = new Color(0, 0, 0, 0);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
