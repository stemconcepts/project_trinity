using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssemblyCSharp
{
    public class ToolTipBehaviour : MonoBehaviour
    {
        public RectTransform rectTransform;
        public RectTransform canvas;
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
                camera = ExploreManager.explorerCamera;
            } else
            {
                camera = Camera.main;
            }
            Vector2 position = camera.ScreenToWorldPoint(Input.mousePosition);
            float pivotX = (position.x / Screen.width) * 100;
            float pivotY = (position.y / Screen.height) * 100;
            var x = (int)Math.Round(pivotX);
            var y = (int)Math.Round(pivotY);
            y = y > 1 ? 1 : y < 0 ? 0 : y;
            x = x > 1 ? 1 : x < 0 ? 0 : x;
            rectTransform.pivot = new Vector2(x, y);
            rectTransform.position = position;
        }
    }
}