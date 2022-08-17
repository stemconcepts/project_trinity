using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts.Helpers.Utility
{
    internal class BoxColliderGenerator : MonoBehaviour
    {
        BoxCollider2D boxCollider;

        /// <summary>
        /// Attempts to set collision box around image
        /// </summary>
        void SetCollisionBox()
        {
            var i = gameObject.GetComponent<Image>();
            if (i && boxCollider)
            {
                boxCollider.size = i.sprite.bounds.size;
            }
        }

        private void Start()
        {
            boxCollider = gameObject.GetComponent<BoxCollider2D>();
            if (!boxCollider)
            {
                boxCollider = gameObject.AddComponent<BoxCollider2D>();
            }
            //SetCollisionBox();
        }
    }
}
