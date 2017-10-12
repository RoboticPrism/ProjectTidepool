using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotationPreview : MonoBehaviour {

    public Player player;
    public Image coloredPreviewImage;
    public Image uncoloredPreviewImage;
    public Button upButton;
    public Button downButton;
    public Button leftButton;
    public Button rightButton;
    public Creature.rotations selectedRotation = Creature.rotations.UP;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetSegment(Segment segment, Color color)
    {
        if (segment.coloredSprite)
        {
            coloredPreviewImage.sprite = segment.coloredSprite;
            coloredPreviewImage.color = color;
        } else
        {
            coloredPreviewImage.sprite = null;
            coloredPreviewImage.color = new Color(0, 0, 0, 0);
        }
        if (segment.uncoloredSprite)
        {
            uncoloredPreviewImage.sprite = segment.uncoloredSprite;
            uncoloredPreviewImage.color = Color.white;
        } else
        {
            uncoloredPreviewImage.sprite = null;
            uncoloredPreviewImage.color = new Color(0, 0, 0, 0);
        }
    }

    public void SetDirection(Creature.rotations currentRotation)
    {
        coloredPreviewImage.gameObject.transform.localEulerAngles = new Vector3(0, 0, Creature.RotationToInt(currentRotation));
        uncoloredPreviewImage.gameObject.transform.localEulerAngles = new Vector3(0, 0, Creature.RotationToInt(currentRotation));
    }
}
