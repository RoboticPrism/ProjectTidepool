using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MousePlacer : MonoBehaviour {
	public int type;
	public Sprite coloredSprite;
    public Sprite uncoloredSprite;
    public SpriteRenderer coloredRenderer;
    public SpriteRenderer uncoloredRenderer;
	public Color color;
    public Player player;

	// Use this for initialization
	void Start () {
        DrawSegment();
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
		if (player.build && Mathf.RoundToInt(mouse.x) < player.width + 1 &&
            Mathf.RoundToInt(mouse.x) > -player.width - 1 &&
            Mathf.RoundToInt(mouse.y) < player.height + 1 &&
            Mathf.RoundToInt(mouse.y) > -player.height - 1) {
            DrawSegment();
        } else {
			coloredRenderer.sprite = null;
            coloredRenderer.color = new Color(0, 0, 0, 0);
            uncoloredRenderer.sprite = null;
            uncoloredRenderer.color = new Color(0, 0, 0, 0);
		}
	}

    public void SetSegment(Segment segment, Color newColor)
    {
        coloredSprite = segment.coloredSprite;
        uncoloredSprite = segment.uncoloredSprite;
        color = newColor;
    }

    public void SetDirection(Creature.rotations currentRotation)
    {
        this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, Creature.RotationToInt(player.buildRotation)));
    }

    void DrawSegment()
    {
        if (coloredSprite)
        {
            coloredRenderer.color = color;
            coloredRenderer.sprite = coloredSprite;
        }
        else
        {
            coloredRenderer.color = new Color(0, 0, 0, 0);
            coloredRenderer.sprite = null;
        }
        if (uncoloredSprite)
        {
            uncoloredRenderer.color = Color.white;
            uncoloredRenderer.sprite = uncoloredSprite;
        }
        else
        {
            uncoloredRenderer.color = new Color(0, 0, 0, 0);
            uncoloredRenderer.sprite = null;
        }
    }
}
