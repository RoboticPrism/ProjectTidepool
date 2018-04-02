using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Abstract class for each piece of a creature in the game
public abstract class Segment : MonoBehaviour{
    [HideInInspector]
    public Creature creature = null;
    [Header("Sprites")]
    public Sprite coloredSprite;
    public Sprite uncoloredSprite;
    [Header("Sprite Renderers")]
    public SpriteRenderer coloredSpriteRenderer;
    public SpriteRenderer uncoloredSpriteRenderer;
    [Header("Attributes")]
    public int pointCost;
    public int healthBonus;
    public int energyBonus;
    public int speedBonus;
    public int weightBonus;
    public int level;
    [Header("Build Settings")]
    public bool multidirectional;
    public bool attachedToCore = true;
    public bool edible;
    public float maxLifespan = 900;
    private float lifespan;
    [Header("Stimulus Attributes")]
    public int threat;

    public void Start()
    {
        lifespan = maxLifespan;
        coloredSpriteRenderer = GetComponent<SpriteRenderer>();
        if(transform.childCount > 0)
        {
            uncoloredSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        }
    }

    public void FixedUpdate()
    {
        if (creature == null)
        {
            if (!edible || lifespan <= 0)
            {
                Destroy(this.gameObject);
            }
            else
            {
                lifespan--;
                if(lifespan < maxLifespan/4)
                if (coloredSpriteRenderer)
                {
                    coloredSpriteRenderer.color = new Color(
                        coloredSpriteRenderer.color.r,
                        coloredSpriteRenderer.color.g,
                        coloredSpriteRenderer.color.b,
                        lifespan / (maxLifespan / 4)
                    );
                }
                if (uncoloredSpriteRenderer)
                {
                    uncoloredSpriteRenderer.color = new Color(
                        uncoloredSpriteRenderer.color.r,
                        uncoloredSpriteRenderer.color.g,
                        uncoloredSpriteRenderer.color.b,
                        lifespan / (maxLifespan / 4)
                    );
                }
            }
        }
    }

    public void UpdateColor(Color newColor)
    {
        if (coloredSprite && coloredSpriteRenderer)
        {
            coloredSpriteRenderer.color = newColor;
        }
    }
}
