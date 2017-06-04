using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Abstract class for each piece of a creature in the game
public abstract class Segment : MonoBehaviour{
	public Creature creature = null;
    public Sprite sprite;
    public int pointCost;
    public int healthBonus;
    public int speedBonus;
    public int weightBonus;
    public bool useColor;
    public bool multidirectional;
    public bool attachedToCore = true;
}
