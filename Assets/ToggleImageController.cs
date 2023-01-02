using AssemblyCSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleImageController : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Image image;
    public List<Sprite> allowedPathSprites = new List<Sprite>();

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
                if (spriteRenderer.sprite)
                {
                    spriteRenderer.material.mainTexture = spriteRenderer.sprite.texture;
                }
            }
            else
            {
                image.sprite = allowedPathSprites[ExploreManager.gameManager.ReturnRandom(allowedPathSprites.Count)];
                if (image.sprite)
                {
                    image.material.mainTexture = image.sprite.texture;
                }
            }
        }
    }

    void Start()
    {
        ChoosePathSprite();
    }
}
