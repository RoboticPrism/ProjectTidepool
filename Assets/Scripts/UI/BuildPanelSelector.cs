using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildPanelSelector : MonoBehaviour {

    public enum categories  {MAIN, ATTACK, DEFENSE, MOVEMENT}
    public List<BuildPanel> buildPanels = new List<BuildPanel>();

	// Use this for initialization
	void Start () {
        ShowPanel(categories.MAIN);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // Sets the given panel category to show and hides the rest
    public void ShowPanel(categories newCategory)
    {
        Debug.Log(buildPanels.Count);
        foreach (BuildPanel panel in buildPanels)
        {
            panel.gameObject.SetActive(panel.category == newCategory);
        }
    }
}
