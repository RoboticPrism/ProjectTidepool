using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildPanelSelectorButton : MonoBehaviour {

    private Player player;
    private BuildPanelSelector buildPanelSelector;
    public bool useColor = true;
    public Sprite image;
    public Button buttonObject;
    public BuildPanel buildPanel;

    // Use this for initialization
    void Start () {
        player = FindObjectOfType<Player>();
        buildPanelSelector = FindObjectOfType<BuildPanelSelector>();
        setImage(image);
        buttonObject.onClick.AddListener(() => buildPanelSelector.ShowPanel(this));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setImage(Sprite newImage)
    {
        image = newImage;
        buttonObject.image.sprite = newImage;
        if (useColor)
        {
            buttonObject.image.color = player.color;
        }
    }
}
