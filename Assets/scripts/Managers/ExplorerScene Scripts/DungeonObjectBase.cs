using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class DungeonObjectBase : MonoBehaviour
    {
        public int position;
        public SpriteRenderer spriteRenderer;
        public List<Sprite> allowedPathSprites;

        // Use this for initialization
        void Start()
        {
            ChoosePathSprite();
        }

        void ChoosePathSprite()
        {
            if (allowedPathSprites.Count > 0)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = allowedPathSprites[ExploreManager.gameManager.ReturnRandom(allowedPathSprites.Count)];
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}