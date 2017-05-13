using UnityEngine;
using System.Collections;

public class BuildCategories : MonoBehaviour {

    public GameObject MainPanel;
    public GameObject AttackPanel;
    public GameObject DefensePanel;
    public GameObject MovePanel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ShowMainPanel()
    {
        MainPanel.gameObject.SetActive(true);
        AttackPanel.gameObject.SetActive(false);
        DefensePanel.gameObject.SetActive(false);
        MovePanel.gameObject.SetActive(false);
    }

    public void ShowAttackPanel()
    {
        MainPanel.gameObject.SetActive(false);
        AttackPanel.gameObject.SetActive(true);
        DefensePanel.gameObject.SetActive(false);
        MovePanel.gameObject.SetActive(false);
    }

    public void ShowDefensePanel()
    {
        MainPanel.gameObject.SetActive(false);
        AttackPanel.gameObject.SetActive(false);
        DefensePanel.gameObject.SetActive(true);
        MovePanel.gameObject.SetActive(false);
    }

    public void ShowMovePanel()
    {
        MainPanel.gameObject.SetActive(false);
        AttackPanel.gameObject.SetActive(false);
        DefensePanel.gameObject.SetActive(false);
        MovePanel.gameObject.SetActive(true);
    }
}
