using AssemblyCSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleImageController : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Image image;
    public List<Sprite> allowedPathSprites;

    /// <summary>
    /// Randomly select a sprite/image from list of sprites on init
    /// </summary>
    void ChoosePathSprite()
    {
        if (allowedPathSprites.Count > 0)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();
            if (spriteRenderer)
            {
                spriteRenderer.sprite = allowedPathSprites[ExploreManager.gameManager.ReturnRandom(allowedPathSprites.Count)];
            } else
            {
                image.sprite = allowedPathSprites[ExploreManager.gameManager.ReturnRandom(allowedPathSprites.Count)];
            }
        }
    }

    void Start()
    {
        ChoosePathSprite();
    }
}