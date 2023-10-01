using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class ToolTipBehaviour : MonoBehaviour
    {
        public RectTransform rectTransform;
        public RectTransform canvas;
        public GameObject title;
        public GameObject desc;

        void Awake()
        {
            if (!canvas)
            {
                canvas = GameObject.Find("Canvas - Tooltip").GetComponent<RectTransform>();
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Camera camera = null;
            if (MainGameManager.instance.SceneManager.currentScene.ToLower() == "exploration")
            {
                camera = MainGameManager.instance.exploreManager.explorerCamera;
            } else
            {
                camera = Camera.main;
            }
            Vector2 position = camera.ScreenToWorldPoint(Input.mousePosition);
            var isaboveMiddleX = (Screen.width / 2) > Input.mousePosition.x;
            var isaboveMiddleY = (Screen.height / 2) > Input.mousePosition.y;
            var y = isaboveMiddleY ? 0 : 1;
            var x = isaboveMiddleX ? 0 : 1;
            //float pivotX = (position.x / Screen.width) * 100;
            //float pivotY = (position.y / Screen.height) * 100;
            //var x = (int)Math.Round(pivotX);
           // var y = (int)Math.Round(pivotY);
            //y = y > 1 ? 1 : y < 0 ? 0 : y;
            //x = x > 1 ? 1 : x < 0 ? 0 : x;
            var descWidth = desc.GetComponent<Text>();
            var titleWidth = title.GetComponent<Text>();
            if (titleWidth)
            {
                this.gameObject.GetComponent<LayoutElement>().preferredWidth = titleWidth.preferredWidth > 600 ? 600 : titleWidth.preferredWidth;
            }
            if (descWidth && descWidth.preferredWidth > titleWidth.preferredWidth)
            {
                this.gameObject.GetComponent<LayoutElement>().preferredWidth = descWidth.preferredWidth > 600 ? 600 : descWidth.preferredWidth;
            }
            rectTransform.pivot = new Vector2(x, y);
            /*if (rectTransform.pivot.x != x || rectTransform.pivot.y != y)
            {
                rectTransform.DOPivot(new Vector2(x, y), 0.5f);
            }*/
            rectTransform.position = Input.mousePosition;
        }
    }
}