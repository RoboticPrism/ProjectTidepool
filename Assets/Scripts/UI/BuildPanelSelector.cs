using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildPanelSelector : MonoBehaviour {

    public BuildPanelSelectorButton mainPanelButton;
    List<BuildPanelSelectorButton> buildPanelSelectorButtons;

	// Use this for initialization
	void Start () {
        buildPanelSelectorButtons = new List<BuildPanelSelectorButton>(FindObjectsOfType<BuildPanelSelectorButton>());
        ShowPanel(mainPanelButton);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // Sets the given panel category to show and hides the rest
    public void ShowPanel(BuildPanelSelectorButton selectedBuildPanelSelectorButton)
    {
        foreach (BuildPanelSelectorButton buildPanelSelectorButton in buildPanelSelectorButtons)
        {
            buildPanelSelectorButton.buildPanel.gameObject.SetActive(buildPanelSelectorButton == selectedBuildPanelSelectorButton);
        }
    }
}
