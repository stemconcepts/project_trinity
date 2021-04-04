using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class ToolTipBehaviour : MonoBehaviour
    {
        public RectTransform rectTransform;
        void Awake()
        {
            //rectTransform = GetComponent<RectTransform>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float pivotX = position.x / Screen.width;
            float pivotY = position.y / Screen.height;
            rectTransform.pivot = new Vector2(pivotX, pivotY);
            transform.position = position;
        }
    }
}