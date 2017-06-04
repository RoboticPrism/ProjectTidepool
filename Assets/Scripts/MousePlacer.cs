using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MousePlacer : MonoBehaviour {
	public int type;
	public Sprite sprite;
	public Color color;
	public Player player;
	// Use this for initialization
	void Start () {
		GetComponent<SpriteRenderer> ().color = player.playerColor;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 mouse=Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
		float x = mouse.x;
		float y = mouse.y;
		float rot = Mathf.PI * player.transform.rotation.eulerAngles.z / 180;
		this.transform.localPosition=new Vector3(
			Mathf.RoundToInt (x * Mathf.Cos(rot) + y * Mathf.Sin(rot)),
			Mathf.RoundToInt (-x * Mathf.Sin(rot) + y * Mathf.Cos(rot)),
			0);
		this.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, player.rot));
		if (player.build && Mathf.RoundToInt(mouse.x) < player.width + 1 &&
            Mathf.RoundToInt(mouse.x) > -player.width - 1 &&
            Mathf.RoundToInt(mouse.y) < player.height + 1 &&
            Mathf.RoundToInt(mouse.y) > -player.height - 1) {
			GetComponent<SpriteRenderer> ().color = color;
			GetComponent<SpriteRenderer> ().sprite = sprite;
		} else {
			GetComponent<SpriteRenderer> ().sprite = null;
		}
	}
}
