using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Abstract class for each piece of a creature in the game
public abstract class Segment : MonoBehaviour{
	public Creature creature = null;
    public Sprite coloredSprite;
    public Sprite uncoloredSprite;
    public int pointCost;
    public int healthBonus;
    public int energyBonus;
    public int speedBonus;
    public int weightBonus;
    public int level;
    public bool multidirectional;
    public bool attachedToCore = true;
    public bool edible;
    public int threat;

    public void FixedUpdate()
    {
        if (creature == null && !edible)
        {
            Destroy(this.gameObject);
        }
    }
}
