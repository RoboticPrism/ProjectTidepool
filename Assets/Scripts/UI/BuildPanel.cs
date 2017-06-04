using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildPanel : MonoBehaviour {

    public Sprite image;
    public List<BuildButton> buildButtons = new List<BuildButton>();

    private float padding = 0.05f;
    private float height = 0.2f;

	// Use this for initialization
	void Start () {
        int index = 1;
        foreach (BuildButton buildButton in buildButtons)
        {
            RectTransform rect = buildButton.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(padding, 1 - ((padding * index) + (height*(index))));
            rect.anchorMax = new Vector2(1 - padding, 1 - ((padding * index) + (height * (index - 1))));
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 0);
            index++;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
